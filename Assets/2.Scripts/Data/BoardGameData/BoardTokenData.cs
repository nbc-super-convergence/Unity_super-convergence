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
    //public Vector3 position;
    //public float rotY;

    //public string id;
    public int keyAmount; //재화, 이름 언제든지 수정가능
    public int trophyAmount;

    //public PlayerTokenState state;
    public BoardGameData gameData;

    public BoardTokenData(UserInfo userInfo)
    {
        this.userInfo = userInfo;
        keyAmount = 10;
        trophyAmount = 0;
        gameData = new();
    }
}