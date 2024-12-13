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

        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    //필요한것 : Dart의 방향이 움직이는 동기화, 판도 서버에서 동기화 시키기

    public void DartMinigameStartNotification(GamePacket gamePacket)
    {
        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameDart>().GameStart();

        Debug.Log(gamePacket.DartMiniGameStartNotification);
    }

    public void DartGameThrowNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameThrowNotification;
        Debug.Log(response);

        string myId = GameManager.Instance.myInfo.SessionId;
        int idx = GameManager.Instance.myInfo.Color;
        foreach(var dart in response.Result)
        {
            if(GameManager.Instance.SessionDic[dart.SessionId].SessionId.Equals(myId))
            {
                Debug.Log($"{GameManager.Instance.myInfo.Nickname} : 내가 던졌다.");
                MinigameManager.Instance.GetMap<MapGameDart>().DartOrder[idx].AnotherShoot(dart.Power);
            }
        }
    }

    public void DartGameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameOverNotification;
        Debug.Log(response);
    }

    public void DartPannelSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartPannelSyncNotification;
        Debug.Log(response);
    }

    public void DartSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartSyncNotification;
        Debug.Log(response);
    }
}
