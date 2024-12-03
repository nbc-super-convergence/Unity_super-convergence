using System;
using UnityEngine;

public enum PlayerTokenState
{
    idle = 0,
    move = 1,
    die = 2,
}

[Serializable]
public class BoardTokenData
{
    public UserInfo userInfo;
    public Vector3 position;
    public float rotY;

    public string id;
    public int keyAmount; //��ȭ, �̸� �������� ��������
    public int trophyAmount;

    public PlayerTokenState state;
    public BoardGameData gameData;
}
