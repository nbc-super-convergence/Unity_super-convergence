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
    public int keyAmount; //재화, 이름 언제든지 수정가능
    public int trophyAmount;
}
