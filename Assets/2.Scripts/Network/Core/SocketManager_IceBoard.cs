public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //201 : IceMiniGameStartRequest
    //Send 위치 : GameManager or BoardManager

    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {//202
        var response = gamePacket.IceMiniGameReadyNotification;

        //ReadyPanel
        UIManager.Show<UIMinigameReady>(); 

        //데이터 설정, 맵 설정, BGM 설정
        MinigameManager.Instance.SetMiniGame<GameIceSlider>();

        foreach (var p in response.Players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.Controller.SetPos(ToVector3(p.Position));
            miniToken.Controller.SetRotY(p.Rotation);
        }
    }

    //203 : IceGameReadyRequest
    //Send 위치 : UIMinigameReady? UIMiniDesc?

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//204
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI와 연계
        string readyId = response.SessionId;
    }

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//205
        var response = gamePacket.IceMiniGameStartNotification;
        
        //ReadyUI 숨기기
    }

    //206 : IcePlayerSyncRequest
    //Send 위치 : MiniToken

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        miniToken.Controller.SetPos(ToVector3(response.Position));
        miniToken.Controller.SetRotY(response.Rotation);
        miniToken.MiniData.CurState = response.State;
    }

    //208 : IcePlayerDamageRequest
    //Send 위치 : MapFloor?

    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDamageNotification;

        //Player에게 데미지 주기
        //MinigameManager.Instance.GameData.GivePlayerDamage(eGameType.GameIceSlider, response.SessionId, 1);
    }

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IcePlayerDeathNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        //TODO : 사망 로직
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceGameOverNotification;

        //게임 종료 이벤트 + 판넬
        foreach (var r in response.Ranks)
        {
            //PlayerId받아서 Rank 표시.
        }

        //기존 맵 삭제, MiniToken은 삭제x.
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//212
        //var response = gamePacket.IceMapSyncNotification;
        //맵 사이즈 줄이기.
    }
}