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
        onInputDetected += MyHandleInput;
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
        if(curQueueInfo != null)
        {
            curQueueInfo.Clear();
            foreach(var b in successQueue)  
            {
                PoolManager.Instance.Release(b);
            }
            successQueue.Clear();
        }

        if (queuePool.Count != 0)
        {
            curQueueInfo = queuePool.Dequeue();

            if (!isFirst)
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
            GamePacket packet = new();
            packet.DanceTableCompleteRequest = new()
            {
                SessionId = SessionId,
                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()   // 테스트하면서 알아보기
            };
            SocketManager.Instance.OnSend(packet);

            completeText.gameObject.SetActive(true);
            background.gameObject.SetActive(false);
            token.InputHandler.DisablePlayerInput();

        }

        Queue<ArrowBubble> queue = new();
        foreach ( var info in curQueueInfo)
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
    public event Action<float, bool> onInputDetected;

    private Queue<ArrowBubble> successQueue = new();


    /* 407 C2S_DanceKeyPressRequest*/
    public async void OnActionInput(int dir)
    {
        token.InputHandler.DisablePlayerInput();
        if (isClient)
        {
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
                onInputDetected?.Invoke(dir, true);
            }
            token.InputHandler.EnablePlayerInput();
        }
    }

    /* 408 */
    public void OnEventInput(bool isCorrect)
    {
        onInputDetected?.Invoke(tokenData.arrowInput, isCorrect);
    }

    public void MyHandleInput(float inputData, bool isFail)
    {
        if(!isFail)
        {
            CheckInput(inputData, GameManager.Instance.myInfo.SessionId);
        }
    }

    /* 409 */
    public void OtherHandleInput(bool isCorrenct, string sessionId)
    {
        CheckInput(curQueueInfo.Peek().Rotation, sessionId);
    }

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
                token.GetAnimator().Play(newState.GetHashCode(), -1);
            }
            else
            {
                tokenData.CurState = newState;
            }
            AnimState.TriggerDanceAnimation(token.GetAnimator(), newState);

            // 보드 상호작용
            PopBubble();
            //UIManager.Get<UICommandBoardHandler>().boardDic[GameManager.Instance.myInfo.SessionId].OnActionInput(tokenData.arrowInput);
        }
        else
        {
            // 실패
            tokenData.CurState = State.DanceFail;
            AnimState.TriggerDanceAnimation(token.GetAnimator(), State.DanceFail);
            StartCoroutine(FailInput());
        }

        if(curQueueInfo.Count == 0)
        {
            MakeNextBoard();
        }
    }

    public void PopBubble()
    {
        var b = curCommandQueue.Dequeue();
        successQueue.Enqueue(b);
        curQueueInfo.Dequeue();
        b.PlayEffect();
    }

    public IEnumerator FailInput()
    {
        Debug.Log("입력이 틀렸습니다.");
        
        
        // 토큰 효과 재생
        isFail = true;
        failImage.gameObject.SetActive(true);
        token.InputHandler.DisablePlayerInput();
        yield return new WaitForSeconds(1.5f);
        token.InputHandler.EnablePlayerInput();
        isFail = false;
        failImage.gameObject.SetActive(false);
    }
    #endregion
}