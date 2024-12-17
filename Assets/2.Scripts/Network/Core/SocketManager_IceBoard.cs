using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    /* 201 */
    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMiniGameReadyNotification;
        
        UIManager.Hide<BoardUI>();
        //ReadyPanel 띄우기.
#pragma warning disable CS4014 
        UIManager.Show<UIMinigameReady>(eGameType.GameIceSlider);
#pragma warning restore CS4014

        //데이터 설정, 맵 설정, BGM 설정
        MinigameManager.Instance.SetMiniGame<GameIceSlider>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
    }

    /* 202 : IceGameReadyRequest
     * Send 위치 : UIMinigameReady */

    //203
    public void IceGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI와 연계
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    /* 204 */
    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {
        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().GameStart();
    }

    //205 : IcePlayerSyncRequest
    //Send 위치 : MiniToken

    //206
    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSyncNotification;

        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(response.SessionId);
        miniToken.MiniData.nextPos = ToVector3(response.Position);
        miniToken.MiniData.rotY = response.Rotation;
        miniToken.MiniData.CurState = response.State;
    }

    //207 : IcePlayerDamageRequest
    //Send 위치 : MapGameIceSlider

    //208
    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDamageNotification;

        //Player 데미지 이벤트
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().GiveDamage(response.SessionId, 1);
    }

    //209
    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDeathNotification;

        //플레이어 사망 이벤트
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().PlayerDeath(response.SessionId);
    }

    //210
    public void IceGameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceGameOverNotification;

        /*필요 데이터 파싱*/
        List<(int Rank, string SessionId)> rankings = new();
        foreach (var r in response.Ranks)
        {
            rankings.Add((r.Rank_, r.SessionId));
        }

        //UI Minigame Result 판넬 호출
        MinigameManager.Instance.curMiniGame.GameEnd(rankings, response.EndTime);
        
        //미니게임 맵 삭제
        MinigameManager.Instance.boardCamera.SetActive(true);
        Destroy(MinigameManager.Instance.curMap.gameObject);
    }

    //211
    public void IceMapSyncNotification(GamePacket gamePacket)
    {
        //맵 작아지는 이벤트
        MinigameManager.Instance.GetMiniGame<GameIceSlider>().MapChangeEvent();
    }
}