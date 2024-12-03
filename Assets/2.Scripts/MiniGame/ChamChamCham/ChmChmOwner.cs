using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChmChmOwner : MonoBehaviour
{
    private Vector3 lookingDirection = Vector3.zero;    //현재 바라보는 방향
    [SerializeField] private List<ChmChmOther> checkSame;   //Other와 방향이 같은지

    //서버에서 받아야 할것들
    //플레이어 Color : Owner가 몇P인지 결정해야 되서
    //총 라운드 회수 : 3회로 생각하고 있는데
    //결정시간 : 제시간 안에 보는 쪽으로 빨리 정하기 최소 3초(?)
    public int Round = 0;
    private float curTime = 0f;

    /// <summary>
    /// 결과를 검사
    /// </summary>
    private void CheckResult()
    {
        //있는 플레이어들의 상태를 검사
        foreach(ChmChmOther other in checkSame)
        {
            if(other.OwnerSame(lookingDirection))
            {
                other.PlayerDead();
                checkSame.Remove(other);    //끝나면 이자리에서 제거
            }
        }

        Round++;

        //라운드 초과시
        if(Round > 3)
        {
            if (checkSame.Count == 0)
            {
                //You Win
            }
            else
            {
                //You Lose
            }
        }
    }
}
