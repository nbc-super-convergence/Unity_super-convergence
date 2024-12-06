using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandBoard : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject prefabBubble;
    [SerializeField] private Image failImage;


    //[SerializeField] private Queue<Queue<ArrowBubble>> commandQueuePool;
    [SerializeField] private Queue<ArrowBubble> curCommandQueue;

    private Queue<Queue<BubbleInfo>> queuePool;
    private Queue<BubbleInfo> curQueueInfo;

    public int numOfbubbles = -1;
    public int round = 0;

    public void Init()
    {
        onInputDetected += HandleInput;

        queuePool = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GetCommandInfoPool();
        MakeNextBoard();
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
            foreach(var b in successQueue)   // curCommandQueue 에 들어있는게 없어서 진입 못함.
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

    // S2C세팅에서 호출하기
    public void SetQueuePool(Queue<Queue<BubbleInfo>> pool)
    {
        queuePool = new(pool);
    }


    #region 플레이

    public bool isFail = false;
    public event Action<float> onInputDetected;

    private Queue<ArrowBubble> successQueue = new();

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            onInputDetected?.Invoke(0f);
            Debug.Log("input : W");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            onInputDetected?.Invoke(90f);
            Debug.Log("input : A");

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            onInputDetected?.Invoke(180f);
            Debug.Log("input : S");

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            onInputDetected?.Invoke(270f);
            Debug.Log("input : D");

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
            PopBubble();
        }
        else
        {
            // 실패
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
        yield return new WaitForSeconds(1.5f);
        isFail = false;
        failImage.gameObject.SetActive(false);
    }
    #endregion

}
