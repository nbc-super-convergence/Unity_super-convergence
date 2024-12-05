using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class CommandBoard : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject prefabBubble;

    //[SerializeField] private Queue<Queue<ArrowBubble>> commandQueuePool;
    [SerializeField] private Queue<ArrowBubble> curCommandQueue;

    private Queue<Queue<BubbleInfo>> queuePool;
    private Queue<BubbleInfo> curQueueInfo;

    public int numOfbubbles = -1;
    public int round = 0;

    public void Init()
    {
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
    }


    // 정보에 따라 방향방울 만들기
    private Queue<ArrowBubble> MakeCommandQueue()
    {
        if(curQueueInfo != null)
        {
            curQueueInfo.Clear();
            foreach(var b in curCommandQueue)
            {
                PoolManager.Instance.Release(b);
            }
            curCommandQueue.Clear();
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
            var bubble = PoolManager.Instance.SpawnFromPool<ArrowBubble>("ArrowBubble");
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
    public void PopBubble()
    {
        //curCommandQueue
    }
    #endregion

}
