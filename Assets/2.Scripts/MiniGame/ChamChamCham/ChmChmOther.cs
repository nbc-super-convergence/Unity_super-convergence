using UnityEngine;

public class ChmChmOther : ChmChmBase
{
    //서버에서 받아야 할것들
    //플레이어 Color : Owner가 몇P인지 결정해야 되서

    public bool IsDead { get; private set; }

    /// <summary>
    /// Owner와 방향이 같은지
    /// </summary>
    /// <param name="dir">Owner의 방향</param>
    /// <returns>같으면 true</returns>
    public bool OwnerSame(Vector3 dir)
    {
        return LookDirection.Equals(dir);
    }

    public void PlayerDead()
    {
        //죽는 모션 구현
        IsDead = true;
    }
}