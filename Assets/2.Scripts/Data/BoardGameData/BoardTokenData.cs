using System;

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
    //public BoardGameData gameData;
    //public Vector3 position;
    //public float rotY;

    //public string id;
    public int coin; //재화
    //public int trophyAmount;

    //public PlayerTokenState state;

    public BoardTokenData(UserInfo userInfo)
    {
        this.userInfo = userInfo;
        coin = 10;
        //trophyAmount = 0;
        //gameData = new();
    }
}