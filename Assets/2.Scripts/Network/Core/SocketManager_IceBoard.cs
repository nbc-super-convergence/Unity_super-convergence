public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //201 : IceMiniGameStartRequest
    //Send ��ġ : GameManager or BoardManager

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//202
        var response = gamePacket.IceMiniGameStartNotification;

        foreach (var p in response.Players)
        {
            //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(p.PlayerType);
            //miniToken.transform.position = ToVector3(p.Position);
            //miniToken.Controller.RotateY(p.Rotation);
        }

        //MinigameManager�� �̵�.
        long readyTime = response.StartTime;
    }

    //203 : IceGameReadyRequest
    //Send ��ġ : UIMinigameReady? UIMiniDesc?

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//204
        var response = gamePacket.IceGameReadyNotification;

        //UI�� ����
        //int readyId = response.PlayerId;
    }

    public void IceGameStartNotification(GamePacket gamePacket)
    {//205
        var response = gamePacket.IceGameStartNotification;

        //MinigameManager�� �̵�. 
        long gameTime = response.StartTime;
    }

    //206 : IcePlayerMoveRequest
    //Send ��ġ : MiniToken

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.PlayerId);
        //miniToken.transform.position = ToVector3(response.Position);
        //miniToken.Controller.RotateY(response.Rotation);
        //miniToken.MiniData.CurState = response.State;

        ////���� ����
        //int curHp = response.Hp;
    }

    //208 : IcePlayerDamageRequest
    //Send ��ġ : MapFloor?

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDeathNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.PlayerId);
        //miniToken.transform.position = ToVector3(response.Position);
        //TODO : ��� ����
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IceGameOverNotification;

        //���� ���� �̺�Ʈ + �ǳ�
        foreach (var r in response.Ranks)
        {
            //PlayerId�޾Ƽ� Rank ǥ��.
        }
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceMapSyncNotification;
    }
}
