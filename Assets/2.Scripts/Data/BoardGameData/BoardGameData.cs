using System;
using System.Collections.Generic;

[Serializable]
public class BoardGameData
{
    public int diceCount = 0; // 지금까지 움직인 수
    public int purchaseArea = 0; //지금까지 일반 땅을 구매한 수 
    public int sellArea = 0; //인수 당한 수
    public int payment = 0; //세금을 지불한 수
    public int Tax = 0; //세금을 지불한 양
    public int arriveMineArea = 0; //내 땅에 도착한 수
    public int loseMiniGame = 0; //미니게임 패배수
    public int WinMiniGame = 0; //미니게임 승리수
    public int highSaveCoin = 0; //게임 중 최대 코인 보유수
}


public interface IGameResult
{
    public List<int> Result();
}