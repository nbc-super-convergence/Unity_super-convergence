using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSlidingDamage : MonoBehaviour
{
    public int PlayerHP { get; private set; } = 100;

    //Ư�� ��ġ �ȿ� ������ ������ ����

    public void GetDamage(int dmg)
    {
        PlayerHP = Mathf.Max(0, PlayerHP - dmg);
    }
}
