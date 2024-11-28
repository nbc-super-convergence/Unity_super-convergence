public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//202
        var response = gamePacket.IceMiniGameStartNotification;
    }

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//204
        var response = gamePacket.IceGameReadyNotification;
    }

    public void IceGameStartNotification(GamePacket gamePacket)
    {//205
        var response = gamePacket.IceGameStartNotification;
    }

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;
    }

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDeathNotification;
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IceGameOverNotification;
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceMapSyncNotification;
    }
}
