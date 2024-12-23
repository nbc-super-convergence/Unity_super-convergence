using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandBoard : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private Image background;
    [SerializeField] private GameObject prefabBubble;
    [SerializeField] private Image failImage;
    [SerializeField] private TMP_Text completeText;

    // -- Bubble Management -- //
    private Queue<Queue<BubbleInfo>> queuePool;
    private Queue<BubbleInfo> curQueueInfo;
    private Queue<ArrowBubble> curCommandQueue;
    private Queue<ArrowBubble> successQueue = new();

    // -- State Flags -- //
    [HideInInspector] public bool isFirst = true;
    [HideInInspector] public bool isFirstInput = false;
    private bool isDisconnected = false;

    // -- Cached References -- //
    private UICourtshipDance uiCourtshipDance;
    private CourtshipDanceData gameData;
    private List<MiniToken> tokens = new();
    private MiniToken myToken;
    [HideInInspector] public List<string> teamSessionIds = new();
    public int TeamNumber { get; private set; }

    // -- Gameplay Resources -- //
    private AudioClip audioClip;
    private TaskCompletionSource<bool> sourceTcs;

    // -- Game Metrics -- //
    [Header("Game Metrics")]
    [SerializeField] private int tableCompleteCount = 0;
    [SerializeField] private long tableCompleteTime = 0;
    [SerializeField] private int numOfbubbles = -1;

    #region Board Control
    public void Init(int teamNumber, Queue<Queue<BubbleInfo>> pool)
    {
        gameData = new();
        TeamNumber = teamNumber;
        queuePool = new(pool);
        foreach(var item in teamSessionIds)
        {
            tokens.Add(MinigameManager.Instance.GetMiniToken(item));
        }
        myToken = MinigameManager.Instance.GetMyToken();

        uiCourtshipDance = UIManager.Get<UICourtshipDance>();
    }

    private void AdjustBackground(int bubbleCount)
    {
        var rt = background.GetComponent<RectTransform>();
        rt.sizeDelta = new(60f + bubbleCount * 100f, rt.sizeDelta.y);
        var rtfailImage = failImage.GetComponent<RectTransform>();
        rtfailImage.sizeDelta = new(60f + bubbleCount * 100f, rtfailImage.sizeDelta.y);
    }

    public void MakeNextBoard()
    {
        curCommandQueue = MakeCommandQueue();
        AdjustBackground(curQueueInfo.Count);
    }

    
    /* 412 */
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

            // 팀전의 경우 팀의 한명에게서만 리퀘스트를 보내게 함.
            if (!isFirst&& !isDisconnected && teamSessionIds[0] == GameManager.Instance.myInfo.SessionId)
            {
                GamePacket packet = new();
                packet.DanceTableCompleteRequest = new()
                {
                    SessionId = teamSessionIds[0],
                    EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()  // 테스트하면서 알아보기
                };
                SocketManager.Instance.OnSend(packet);
            }
            isFirst = false;
            tableCompleteCount++;
            tableCompleteTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        else
        {
            // 테이블 완료
            if (teamSessionIds[0] == GameManager.Instance.myInfo.SessionId)
            {
                GamePacket packet = new();
                packet.DanceTableCompleteRequest = new()
                {
                    SessionId = teamSessionIds[0],
                    EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()   // 테스트하면서 알아보기
                };
                SocketManager.Instance.OnSend(packet);
            }
            completeText.gameObject.SetActive(true);
            background.gameObject.SetActive(false);
            myToken.InputHandler.DisableSimpleInput();
            tableCompleteCount++;
            tableCompleteTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        if (isDisconnected)
        {
            isDisconnected = false;
        }

        Queue<ArrowBubble> queue = new();
        foreach (var info in curQueueInfo)
        {
            var bubble = PoolManager.Instance.Spawn<ArrowBubble>("ArrowBubble");
            bubble.SetArrowBubble(info);
            bubble.transform.SetParent(background.transform);
            bubble.transform.SetAsLastSibling();
            bubble.SetScale();
            queue.Enqueue(bubble);
        }
        numOfbubbles = queue.Count;
        return queue;
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
#endregion

    #region Player Controls
    /* 407 C2S_DanceKeyPressRequest*/
    // 본인 커맨드 보드에서만 호출됨.
    public void OnActionInput(int dir)
    {
        if (!isFirstInput)
        {
            isFirstInput = true;
        }
        GamePacket packet = new();
        packet.DanceKeyPressRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            PressKey = (Direction)dir
        };
        SocketManager.Instance.OnSend(packet);        
    }

    public void MyInputResponse(bool isCorrect, State state)
    {
        if (isCorrect)
        {
            SetAudioClip(state);
            myToken.MiniData.CurState = state;
            AnimState.TriggerDanceAnimation(myToken.GetAnimator(), state);
            PopBubble();
            SoundManager.Instance.PlaySFX(audioClip);
        }
        else
        {
            StartCoroutine(FailInput());
        }
    }

    public void MyTeamNotification(bool isCorrect, State state)
    {
        if (isCorrect)
        {
            SetAudioClip(state);
            foreach(var token in tokens)
            {
                if(token != myToken)
                {
                    token.MiniData.CurState = state;
                    AnimState.TriggerDanceAnimation(token.GetAnimator(), state);
                }
            }
            PopBubble();
            SoundManager.Instance.PlaySFX(audioClip);
        }
        else
        {
            StartCoroutine(FailInput());
        }
    }
        
    // 싱글모드의 잔재
    //private void CheckInput(float rot, string sessionId)
    //{
    //    var target = curQueueInfo.Peek();
    //    if(target.sessionId == sessionId && target.Rotation == rot)
    //    {
    //        State newState = State.DanceWait;
            
    //        switch (target.Rotation)
    //        {
    //            case 0:
    //                newState = State.DanceUp;
    //                break;
    //            case 90:
    //                newState = State.DanceLeft;
    //                break;
    //            case 180:
    //                newState = State.DanceDown;
    //                break;
    //            case 270:
    //                newState = State.DanceRight;
    //                break;
    //        }
    //        SetAudioClip(newState);
    //        if (tokenData.CurState == newState)
    //        {
    //            //token.GetAnimator().Play(newState.GetHashCode(), 1);
    //        }
    //        else
    //        {
    //            tokenData.CurState = newState;
    //        }
    //        AnimState.TriggerDanceAnimation(tokens.GetAnimator(), newState);

    //        // 보드 상호작용
    //        PopBubble();
    //        SoundManager.Instance.PlaySFX(audioClip);
    //    }
    //    else
    //    {
    //        // 실패
    //        tokenData.CurState = State.DanceFail;
    //        SetAudioClip(State.DanceFail);
    //        AnimState.TriggerDanceAnimation(tokens.GetAnimator(), State.DanceFail);
    //        StartCoroutine(FailInput());
    //    }        
    //}
        
    public IEnumerator FailInput()
    {
        failImage.gameObject.SetActive(true);
        SetAudioClip(State.DanceFail);
        foreach (var token in tokens)
        {
            token.InputHandler.isEnable = false;
            token.MiniData.CurState = State.DanceFail;
            AnimState.TriggerDanceAnimation(token.GetAnimator(), State.DanceFail);
        }

        SoundManager.Instance.PlaySFX(audioClip);
        yield return new WaitForSeconds(gameData.stunDelay);

        failImage.gameObject.SetActive(false);
        foreach (var token in tokens)
        {
            token.InputHandler.isEnable = true;            
        }        
    }

    // 노티를 받으면 다른 유저의 보드에서 실행될 메서드
    public void OtherBoardNoti(int teamNumber, bool correct, State state)
    {
        if(teamNumber != TeamNumber)
        {
            Debug.Log("올바르지 않은 접근입니다.");
            return;
        }

        if(!correct)
        {
            foreach( var token in tokens)
            {
                token.MiniData.CurState = State.DanceFail;
                AnimState.TriggerDanceAnimation(token.GetAnimator(), State.DanceFail);
                StartCoroutine(FailInput());
            }
        }
        else
        {
            PopBubble();
            foreach(var token in tokens)
            {
                token.MiniData.CurState = state;
                AnimState.TriggerDanceAnimation(token.GetAnimator(), state);
            }
        }
    }

    private void SetAudioClip(State state)
    {
        switch (state)
        {
            case State.DanceFail:
                audioClip = uiCourtshipDance.sfxClips[4];
                break;
            case State.DanceUp:
                audioClip = uiCourtshipDance.sfxClips[0];
                break;
            case State.DanceDown:
                audioClip = uiCourtshipDance.sfxClips[1];
                break;
            case State.DanceLeft:
                audioClip = uiCourtshipDance.sfxClips[2];
                break;
            case State.DanceRight:
                audioClip = uiCourtshipDance.sfxClips[3];
                break;
        }
    }

    // 플레이 중 누군가 접속을 끊었을 때 실행되는 메서드
    public void ChangeInfoPool(string disconnectedSessionId, string replacementSessionId)
    {
        if (queuePool == null || !teamSessionIds.Contains(disconnectedSessionId) || !teamSessionIds.Contains(replacementSessionId))
            return;

        int replaceColor = GameManager.Instance.SessionDic[replacementSessionId].Color;

        if (curCommandQueue != null)
        {
            for (int i = 0; i < curCommandQueue.Count; i++)
            {
                var b = curCommandQueue.Dequeue();
                b.ColorChange(replaceColor);
                curCommandQueue.Enqueue(b);
            }
        }

        Queue<Queue<BubbleInfo>> tempQueuePool = new Queue<Queue<BubbleInfo>>();

        while (queuePool.Count > 0)
        {
            Queue<BubbleInfo> innerQueue = queuePool.Dequeue();
            Queue<BubbleInfo> tempInnerQueue = new Queue<BubbleInfo>();

            while (innerQueue.Count > 0)
            {
                BubbleInfo bubbleInfo = innerQueue.Dequeue();
                if (bubbleInfo.SessionId == disconnectedSessionId)
                {
                    bubbleInfo.SetSessionId(replacementSessionId);
                    bubbleInfo.SetColor(replaceColor);
                }
                tempInnerQueue.Enqueue(bubbleInfo);
            }

            tempQueuePool.Enqueue(tempInnerQueue);
        }

        queuePool = tempQueuePool;
        isDisconnected = true;
    }
    #endregion
}