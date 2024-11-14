using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSlidingDamage : MonoBehaviour
{
    public int PlayerHP { get; private set; } = 100;

    //특정 위치 안에 있으면 데미지 감소

    public void GetDamage(int dmg)
    {
        PlayerHP = Mathf.Max(0, PlayerHP - dmg);
    }
}
