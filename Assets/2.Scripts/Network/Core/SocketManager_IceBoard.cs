using System.Collections.Generic;
using UnityEngine.Playables;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {//201
        var response = gamePacket.IceMiniGameReadyNotification;

        //ReadyPanel 띄우기.
        UIManager.Show<UIMinigameReady>(eGameType.GameIceSlider); 

        //데이터 설정, 맵 설정, BGM 설정
        MinigameManager.Instance.SetMiniGame<GameIceSlider>();

        foreach (var p in response.Players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.Controller.SetPos(ToVector3(p.Position));
            miniToken.Controller.SetRotY(p.Rotation);
            miniToken.EnableMiniToken();
        }
    }

    //202 : IceGameReadyRequest
    //Send 위치 : UIMinigameReady (완료)

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//203
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI와 연계
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//204
        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().GameStart();
    }

    //206 : IcePlayerSyncRequest
    //Send 위치 : MiniToken (완료)

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        miniToken.Controller.SetNextPos(ToVector3(response.Position));
        miniToken.Controller.SetRotY(response.Rotation);
        miniToken.MiniData.CurState = response.State;
    }

    //TODO
    //208 : IcePlayerDamageRequest
    //Send 위치 : MapGameIceSlider (완료)

    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDamageNotification;

        //Player에게 데미지 주기
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .GiveDamage(response.SessionId, 1);
    }

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IcePlayerDeathNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .PlayerDeath(response.SessionId);
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceGameOverNotification;

        Dictionary<string, int> rankings = new();
        foreach (var r in response.Ranks)
        {
            rankings.Add(r.SessionId, r.Rank_);
        }

        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .GameEnd(rankings);

        //미니게임 맵 삭제
        Destroy(MinigameManager.Instance.CurMap.gameObject); 
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//212
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .MapChangeEvent();
    }
}