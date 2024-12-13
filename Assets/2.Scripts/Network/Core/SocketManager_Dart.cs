using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public void DartMiniGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartMiniGameReadyNotification;
        Debug.Log(response);

        UIManager.Hide<BoardUI>();
#pragma warning disable CS4014 
        UIManager.Show<UIMinigameDart>(eGameType.GameDart);
#pragma warning restore CS4014

        MinigameManager.Instance.SetMiniGame<GameDart>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
    }   
    
    public void DartGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameReadyNotification;
        Debug.Log(response);

        UIManager.Get<UIMinigameDart>().SetNickname(response.SessionId);
    }

    public void DartMinigameStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartMiniGameStartNotification;
        Debug.Log(response);


    }

    public void DartGameThrowNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameThrowNotification;
        Debug.Log(response);

        //사용자가 아닌 상대라면 response의 데이터를 받아 던지기 신호

    }

    public void DartGameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameOverNotification;
        Debug.Log(response);
    }
}
