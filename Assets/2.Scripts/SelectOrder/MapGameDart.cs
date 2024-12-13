using System.Collections.Generic;
using UnityEngine;

public class MapGameDart : MapBase
{
    public GameDartPanel DartPanel { get; private set; }

    //다트그룹
    public List<DartPlayer> DartOrder;

    private int nowPlayer = 0;  // 현재 플레이어 차례

    private void Awake()
    {
        //각 플레이어의 인덱스 설정 (내가 몇P인지 위해)
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
    /// 플레이어 인원만큼 다트에 인덱스 추가
    /// </summary>
    /// <param name="playerCnt"></param>
    public void SetDartPlayers(int playerCnt)
    {
        if (playerCnt < DartOrder.Count)
        {
            for (int i = 0; i < playerCnt; i++)
            {
                DartOrder[i].SetPlayerIndex(i);
            }
        }
        else return;
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