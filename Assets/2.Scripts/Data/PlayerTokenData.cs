using System;
using UnityEngine;

public enum PlayerTokenState
{
    idle = 0,
    move = 1,
    die = 2,
}

[Serializable]
public class PlayerTokenData 
{
    public Vector3 float3;
    public float hp;
    public PlayerTokenState state;
    public int keyAmount; //��ȭ, �̸� �������� ��������
    public int trophyAmount;
}
