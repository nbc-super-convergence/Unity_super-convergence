using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChmChmManager : Singleton<ChmChmManager>
{
    public enum YourRole { Owner, Other };
    public YourRole MyRole;

    public ChmChmOwner OwnerPlayer;
    public List<ChmChmOther> OtherPlayers;

    //서버에서 받아야 할것들
    //플레이어 Color : Owner가 몇P인지 결정해야 되서
    //총 라운드 회수 : 3회로 생각하고 있는데
    //결정시간 : 제시간 안에 보는 쪽으로 빨리 정하기 최소 3초(?)
    public int Round = 0;
    private float curTime = 0f;
    private float matchTime = 3f;   //3초까지 방향 선택

    private void Update()
    {

    }

    /// <summary>
    /// 결과를 검사
    /// </summary>
    private void CheckResult()
    {
        //있는 플레이어들의 상태를 검사
        foreach (ChmChmOther other in OtherPlayers)
        {
            if (other.OwnerSame(OwnerPlayer.LookDirection))
            {
                other.PlayerDead();
                OtherPlayers.Remove(other);    //끝나면 이자리에서 제거
            }
        }

        Round++;

        //라운드 초과시
        if (Round > 3)
        {
            if (OtherPlayers.Count == 0)
            {
                Debug.Log("Owner Wins");
            }
            else
            {
                Debug.Log("Others Win");
            }
        }
    }
}
