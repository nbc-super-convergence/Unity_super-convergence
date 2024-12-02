using System.Collections.Generic;
using UnityEngine.Playables;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {//201
        var response = gamePacket.IceMiniGameReadyNotification;

        //ReadyPanel ����.
        UIManager.Show<UIMinigameReady>(eGameType.GameIceSlider); 

        //������ ����, �� ����, BGM ����
        MinigameManager.Instance.SetMiniGame<GameIceSlider>();

        foreach (var p in response.Players)
        {//�̴� ��ū ��ġ �ʱ�ȭ
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.Controller.SetPos(ToVector3(p.Position));
            miniToken.Controller.SetRotY(p.Rotation);
            miniToken.EnableMiniToken();
        }
    }

    //202 : IceGameReadyRequest
    //Send ��ġ : UIMinigameReady (�Ϸ�)

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//203
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI�� ����
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//204
        //ReadyUI �����
        UIManager.Hide<UIMinigameReady>();
        //GameStart �Լ� ȣ��
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().GameStart();
    }

    //206 : IcePlayerSyncRequest
    //Send ��ġ : MiniToken (�Ϸ�)

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
    //Send ��ġ : MapGameIceSlider (�Ϸ�)

    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDamageNotification;

        //Player���� ������ �ֱ�
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

        //�̴ϰ��� �� ����
        Destroy(MinigameManager.Instance.CurMap.gameObject); 
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//212
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .MapChangeEvent();
    }
}