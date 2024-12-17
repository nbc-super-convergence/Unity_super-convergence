using System.Collections.Generic;

public class MapGameDart : MapBase
{
    public GameDartPanel DartPanel;

    //다트그룹
    public List<DartPlayer> DartOrder;

    private int nowPlayer = 0;  // 현재 플레이어 차례

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
    public void BeginSelectOrder()
    {
        DartOrder[nowPlayer].gameObject.SetActive(true);
    }

    public void MovePanel()
    {
        StartCoroutine(DartPanel.MoveCoroutine());
    }
    public void StopPanel()
    {
        DartPanel.isMove = false;
    }
}