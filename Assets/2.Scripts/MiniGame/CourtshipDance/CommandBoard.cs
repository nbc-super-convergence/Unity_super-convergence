using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandBoard : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject prefabBubble;

    [SerializeField] private Queue<Queue<ArrowBubble>> commandQueuePool;
    [SerializeField] private Queue<ArrowBubble> commandQueue;

    private Queue<Queue<BubbleInfo>> queuePool;
    private Queue<BubbleInfo> queueInfo;

    // 백그라운드 아래에 버블이 몇 개 있냐에 따라서 background 크기 조절하기

    private void AdjustBackground()
    {

    }


}
