public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //201 : IceMiniGameStartRequest
    //Send 위치 : GameManager or BoardManager

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//202
        var response = gamePacket.IceMiniGameStartNotification;

        foreach (var p in response.Players)
        {
            //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(p.PlayerType);
            //miniToken.transform.position = ToVector3(p.Position);
            //miniToken.Controller.RotateY(p.Rotation);
        }

        //MinigameManager로 이동.
        long readyTime = response.StartTime;
    }

    //203 : IceGameReadyRequest
    //Send 위치 : UIMinigameReady? UIMiniDesc?

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//204
        var response = gamePacket.IceGameReadyNotification;

        //UI와 연계
        //int readyId = response.PlayerId;
    }

    public void IceGameStartNotification(GamePacket gamePacket)
    {//205
        var response = gamePacket.IceGameStartNotification;

        //MinigameManager로 이동. 
        long gameTime = response.StartTime;
    }

    //206 : IcePlayerMoveRequest
    //Send 위치 : MiniToken

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.PlayerId);
        //miniToken.transform.position = ToVector3(response.Position);
        //miniToken.Controller.RotateY(response.Rotation);
        //miniToken.MiniData.CurState = response.State;

        ////추후 연계
        //int curHp = response.Hp;
    }

    //208 : IcePlayerDamageRequest
    //Send 위치 : MapFloor?

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDeathNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.PlayerId);
        //miniToken.transform.position = ToVector3(response.Position);
        //TODO : 사망 로직
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IceGameOverNotification;

        //게임 종료 이벤트 + 판넬
        foreach (var r in response.Ranks)
        {
            //PlayerId받아서 Rank 표시.
        }
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceMapSyncNotification;
    }
}
