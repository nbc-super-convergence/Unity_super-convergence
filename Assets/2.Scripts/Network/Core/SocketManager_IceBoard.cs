public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //201 : IceMiniGameStartRequest
    //Send ��ġ : GameManager or BoardManager

    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {//202
        var response = gamePacket.IceMiniGameReadyNotification;

        //ReadyPanel
        UIManager.Show<UIMinigameReady>(); 

        //������ ����, �� ����, BGM ����
        MinigameManager.Instance.SetMiniGame<GameIceSlider>();

        foreach (var p in response.Players)
        {//�̴� ��ū ��ġ �ʱ�ȭ
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.Controller.SetPos(ToVector3(p.Position));
            miniToken.Controller.SetRotY(p.Rotation);
        }
    }

    //203 : IceGameReadyRequest
    //Send ��ġ : UIMinigameReady? UIMiniDesc?

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//204
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI�� ����
        string readyId = response.SessionId;
    }

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//205
        var response = gamePacket.IceMiniGameStartNotification;
        
        //ReadyUI �����
    }

    //206 : IcePlayerSyncRequest
    //Send ��ġ : MiniToken

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        miniToken.Controller.SetPos(ToVector3(response.Position));
        miniToken.Controller.SetRotY(response.Rotation);
        miniToken.MiniData.CurState = response.State;
    }

    //208 : IcePlayerDamageRequest
    //Send ��ġ : MapFloor?

    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDamageNotification;

        //Player���� ������ �ֱ�
        //MinigameManager.Instance.GameData.GivePlayerDamage(eGameType.GameIceSlider, response.SessionId, 1);
    }

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IcePlayerDeathNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        //TODO : ��� ����
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceGameOverNotification;

        //���� ���� �̺�Ʈ + �ǳ�
        foreach (var r in response.Ranks)
        {
            //PlayerId�޾Ƽ� Rank ǥ��.
        }

        //���� �� ����, MiniToken�� ����x.
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//212
        //var response = gamePacket.IceMapSyncNotification;
        //�� ������ ���̱�.
    }
}