using System.Collections.Generic;
using UnityEngine;

public class MapGameDart : MapBase
{
    [SerializeField] private GameDartPanel DartPanel;

    //다트그룹
    public List<DartPlayer> DartOrder;

    private int nowPlayer = 0;  // 현재 플레이어 차례

    private void Awake()
    {
        
    }

    private void Start()
    {
        BeginSelectOrder();
    }

    private void Update()
    {
        if (nowPlayer < DartOrder.Count)
        {
            //내 다트를 받으면 해당 다트의 속성들을 UI에 적용
        }
    }

    /// <summary>
    /// 지금부터 시작
    /// </summary>
    private void BeginSelectOrder()
    {
        DartOrder[nowPlayer].gameObject.SetActive(true);
    }

    public void StopPanel()
    {
        DartPanel.isMove = false;
    }
}