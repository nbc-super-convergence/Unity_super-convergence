using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandBoard : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject prefabBubble;
    [SerializeField] private Image failImage;
    [SerializeField] private TMP_Text completeText;

    [SerializeField] private Queue<ArrowBubble> curCommandQueue;

    private Queue<Queue<BubbleInfo>> queuePool;
    private Queue<BubbleInfo> curQueueInfo;

    private bool isClient = false;
    private MiniToken token;
    private MiniTokenData tokenData;
    public string SessionId { get; private set; }
    public int TeamId { get; private set; }
    public int numOfbubbles = -1;
    public int round = 0;

    private TaskCompletionSource<bool> sourceTcs;

    public void TrySetTask(bool isSuccess)
    {
        bool b = sourceTcs.TrySetResult(isSuccess);
    }

    public void Init()
    {
        //onInputDetected += MyHandleInput;
        if(SessionId == GameManager.Instance.myInfo.SessionId)
        {
            isClient = true;
        }
        token = MinigameManager.Instance.GetMiniToken(SessionId);
        tokenData = token.MiniData;
    }
    public void SetSessionId(string sessionId)
    { SessionId = sessionId; }
    public void SetTeamId(int teamId)
    {  TeamId = teamId; }

    public void SetPool(Queue<Queue<BubbleInfo>> pool)
    {
        queuePool = new(pool);
    }

    public void MakeNextBoard()
    {
        curCommandQueue = MakeCommandQueue();
        AdjustBackground(curQueueInfo.Count);
        round++;
    }

    private void AdjustBackground(int bubbleCount)
    {
        var rt = background.GetComponent<RectTransform>();
        rt.sizeDelta = new(60f + bubbleCount * 100f, rt.sizeDelta.y);
        var rtfailImage = failImage.GetComponent<RectTransform>();
        rtfailImage.sizeDelta = new(60f + bubbleCount * 100f, rtfailImage.sizeDelta.y);
    }

    /* 412 */
    // 정보에 따라 방향방울 만들기
    bool isFirst = true;
    private Queue<ArrowBubble> MakeCommandQueue()
    {

        if (curQueueInfo != null)
        {
            curQueueInfo.Clear();
            foreach (var b in successQueue)
            {
                PoolManager.Instance.Release(b);
            }
            successQueue.Clear();
        }

        if (queuePool.Count != 0)
        {
            curQueueInfo = queuePool.Dequeue();

            if (!isFirst && isClient)
            {
                GamePacket packet = new();
                packet.DanceTableCompleteRequest = new()
                {
                    SessionId = SessionId,
                    EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()  // 테스트하면서 알아보기
                };
                SocketManager.Instance.OnSend(packet);
            }
            isFirst = true;
        }
        else
        {
            // 완료 로직
            // 완료 효과
            if (isClient)
            {
                GamePacket packet = new();
                packet.DanceTableCompleteRequest = new()
                {
                    SessionId = SessionId,
                    EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()   // 테스트하면서 알아보기
                };
                SocketManager.Instance.OnSend(packet);
            }
            completeText.gameObject.SetActive(true);
            background.gameObject.SetActive(false);
            //token.InputHandler.DisablePlayerInput();
        }

        Queue<ArrowBubble> queue = new();
        foreach (var info in curQueueInfo)
        {
            var bubble = PoolManager.Instance.Spawn<ArrowBubble>("ArrowBubble");
            bubble.SetArrowBubble(info);
            bubble.transform.SetParent(background.transform);
            bubble.transform.SetAsLastSibling();
            queue.Enqueue(bubble);
        }
        numOfbubbles = queue.Count;
        return queue;
    }

    


    #region 플레이

    public bool isFail = false;
    //public event Action<float, bool> onInputDetected;

    private Queue<ArrowBubble> successQueue = new();


    /* 407 C2S_DanceKeyPressRequest*/
    // 본인 커맨드 보드에서만 호출됨.
    public async void OnActionInput(int dir)
    {
        //token.InputHandler.DisablePlayerInput();
        GamePacket packet = new();
        packet.DanceKeyPressRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            PressKey = (Direction)dir
        };
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);
        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {

        }
        else
        {
            //onInputDetected?.Invoke(dir, true);
        }
    }

    /* 408 */

    public void MyHandleInput(bool isFail)
    {
        CheckInput(tokenData.arrowInput, GameManager.Instance.myInfo.SessionId);
    }

    /* 409 */
   
    private void CheckInput(float rot, string sessionId)
    {
        var target = curQueueInfo.Peek();
        if(target.sessionId == sessionId && target.Rotation == rot)
        {
            // 성공
            // 애니메이션 재생
            //tokenData.CurState = State.DanceIdle;
            State newState = State.DanceWait;
            switch (target.Rotation)
            {
                case 0:
                    newState = State.DanceUp; break;
                case 90:
                    newState = State.DanceLeft; break;
                case 180:
                    newState = State.DanceDown; break;
                case 270:
                    newState = State.DanceRight; break;
            }
            if (tokenData.CurState == newState)
            {
                //token.GetAnimator().Play(newState.GetHashCode(), 1);
            }
            else
            {
                tokenData.CurState = newState;
            }
            AnimState.TriggerDanceAnimation(token.GetAnimator(), newState);

            // 보드 상호작용
            PopBubble();
        }
        else
        {
            // 실패
            tokenData.CurState = State.DanceFail;
            AnimState.TriggerDanceAnimation(token.GetAnimator(), State.DanceFail);
            StartCoroutine(FailInput());
        }        
    }

    public void PopBubble()
    {
        var b = curCommandQueue.Dequeue();
        successQueue.Enqueue(b);
        curQueueInfo.Dequeue();
        b.PlayEffect();

        if (curQueueInfo.Count == 0)
        {
            MakeNextBoard();
        }
    }

    public IEnumerator FailInput()
    {        
        // 토큰 효과 재생
        isFail = true;
        failImage.gameObject.SetActive(true);
        token.InputHandler.isEnable = false;

        yield return new WaitForSeconds(1.5f);
        token.InputHandler.isEnable = true;
        isFail = false;
        failImage.gameObject.SetActive(false);
    }

    // 노티를 받으면 다른 유저의 보드에서 실행될 메서드
    public void OtherBoardNoti(string sessionId, bool correct, State state)
    {
        if(sessionId != this.SessionId)
        {
            Debug.Log("올바르지 않은 접근입니다.");
            return;
        }

        if(!correct)
        {
            tokenData.CurState = State.DanceFail;
            AnimState.TriggerDanceAnimation(token.GetAnimator(), State.DanceFail);
            StartCoroutine(FailInput());
        }
        else
        {
            PopBubble();
            tokenData.CurState = state;
            AnimState.TriggerDanceAnimation(token.GetAnimator(), state);
        }
    }
    #endregion
}