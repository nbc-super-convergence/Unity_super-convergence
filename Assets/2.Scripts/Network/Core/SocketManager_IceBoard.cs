using System;
using System.Collections.Generic;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    /* 201 */
    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMiniGameReadyNotification;

        //ReadyPanel ����.
#pragma warning disable CS4014 
        UIManager.Show<UIMinigameReady>(eGameType.GameIceSlider);
#pragma warning restore CS4014

        //������ ����, �� ����, BGM ����
        MinigameManager.Instance.SetMiniGame<GameIceSlider>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
    }

    /* 202 : IceGameReadyRequest
     * Send ��ġ : UIMinigameReady (�Ϸ�) */

    //203
    public void IceGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI�� ����
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    /* 204 */
    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {
        //ReadyUI �����
        UIManager.Hide<UIMinigameReady>();
        //GameStart �Լ� ȣ��
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().GameStart();
    }

    //205 : IcePlayerSyncRequest
    //Send ��ġ : MiniToken (�Ϸ�)

    //206
    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSyncNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        miniToken.Controller.SetNextPos(ToVector3(response.Position));
        miniToken.Controller.SetRotY(response.Rotation);
        miniToken.MiniData.CurState = response.State;
    }

    //207 : IcePlayerDamageRequest
    //Send ��ġ : MapGameIceSlider (�Ϸ�)

    //208
    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDamageNotification;

        //Player���� ������ �ֱ�
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .GiveDamage(response.SessionId, 1);
    }

    //209
    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDeathNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .PlayerDeath(response.SessionId);
    }

    //210
    public void IceGameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceGameOverNotification;

        Dictionary<string, int> rankings = new();
        foreach (var r in response.Ranks)
        {
            rankings.Add(r.SessionId, r.Rank_);
        }

        DateTime time = DateTimeOffset.FromUnixTimeMilliseconds(response.EndTime).UtcDateTime;

        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .GameEnd(rankings, time);

        //�̴ϰ��� �� ����
        MinigameManager.Instance.boardCamera.SetActive(true);
        Destroy(MinigameManager.Instance.CurMap.gameObject);
    }

    //211
    public void IceMapSyncNotification(GamePacket gamePacket)
    {
        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .MapChangeEvent();
    }

    //212
    public void IcePlayerExitNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerExitNotification;
        GameManager.Instance.DeleteSessionId(response.SessionId);
    }
}