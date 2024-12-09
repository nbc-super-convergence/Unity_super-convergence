using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class CommandBoard : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject prefabBubble;
    [SerializeField] private Image failImage;

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

    public void Init()
    {
        onInputDetected += HandleInput;
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


    // 정보에 따라 방향방울 만들기
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

        if(queuePool.Count != 0)
        {
            curQueueInfo = queuePool.Dequeue();
        }
        else
        {
            // 완료 버블 띄우기? C O M P L E T E
            // return;
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
    public event Action<float> onInputDetected;

    private Queue<ArrowBubble> successQueue = new();

    
    public void OnActionInput(int dir)
    {
        if (isClient)
        {
            onInputDetected?.Invoke(dir);
        }
    }

    public void HandleInput(float inputData)
    {
        if(!isFail)
        {
            CheckInput(inputData, GameManager.Instance.myInfo.SessionId);
        }
    }

    private void CheckInput(float rot, string sessionId)
    {
        var target = curQueueInfo.Peek();
        if(target.sessionId == sessionId && target.Rotation == rot)
        {
            // 성공
            // 애니메이션 재생
            //tokenData.CurState = State.DanceIdle;
            State newState = State.DanceIdle;
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
            tokenData.CurState = State.DanceSlip;
            AnimState.TriggerDanceAnimation(token.GetAnimator(), State.DanceSlip);
            StartCoroutine(FailInput());
        }

        if(curQueueInfo.Count == 0)
        {
            //UIManager.Get<UICommandBoardHandler>().Next();
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
