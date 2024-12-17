using System.Collections.Generic;
using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public void BombMiniGameReadyNotification(GamePacket packet)
    {
        UIManager.Hide<BoardUI>();
        var response = packet.BombMiniGameReadyNotification;

#pragma warning disable CS4014 
        UIManager.Show<UIMinigameReady>(eGameType.GameBombDelivery);
#pragma warning restore CS4014

        MinigameManager.Instance.SetMiniGame<GameBombDelivery>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
    }

    public void BombGameReadyNotification(GamePacket packet)
    {
        var response = packet.BombGameReadyNotification;

        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    public void BombMiniGameStartNotification(GamePacket packet)
    {
        var response = packet.BombMiniGameStartNotification;

        UIManager.Hide<UIMinigameReady>();

        MinigameManager.Instance.GetMiniGame<GameBombDelivery>().GameStart();

    }

    public void BombPlayerSyncNotification(GamePacket packet)
    {
        var response = packet.BombPlayerSyncNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        miniToken.MiniData.nextPos = ToVector3(response.Position);
        miniToken.MiniData.rotY = response.Rotation;
        miniToken.MiniData.CurState = response.State;

        if (miniToken.ServerMoveCoroutine != null)
        {
            StopCoroutine(miniToken.ServerMoveCoroutine);
        }

        miniToken.ServerMoveCoroutine = StartCoroutine(miniToken.ServerMove());
    }

    public void BombPlayerDeathNotification(GamePacket packet)
    {
        var response = packet.BombPlayerDeathNotification;

        var game = MinigameManager.Instance.GetMiniGame<GameBombDelivery>();
        game.Explosion(response.SessionId);

        if(!response.BombSessionId.Equals("NULL"))
            game.SetTarget(response.BombSessionId);
    }

    public void BombMoveNotification(GamePacket packet)
    {
        var response = packet.BombMoveNotification;

        var game = MinigameManager.Instance.GetMiniGame<GameBombDelivery>();
        game.SetTarget(response.SessionId);
    }

    public void BombGameOverNotification(GamePacket packet)
    {
        var response = packet.BombGameOverNotification;

        List<(int Rank, string SessionId)> rankings = new();
        foreach (var r in response.Ranks)
        {
            rankings.Add((r.Rank_, r.SessionId));
        }

        //UI Minigame Result 판넬 호출
        MinigameManager.Instance.curMiniGame.GameEnd(rankings, response.EndTime);

        //미니게임 맵 삭제
        MinigameManager.Instance.boardCamera.SetActive(true);

        MinigameManager.Instance.GetMiniGame<GameBombDelivery>().GameOver();
    }
}
