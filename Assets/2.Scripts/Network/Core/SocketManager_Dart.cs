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

        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    //필요한것 : Dart의 방향이 움직이는 동기화, 판도 서버에서 동기화 시키기

    public void DartMiniGameStartNotification(GamePacket gamePacket)
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

        Dictionary<string, int> rankings = new();
        foreach(var r in response.Ranks)
        {
            rankings.Add(r.SessionId, r.Rank_);
        }

        //UI Minigame Result 판넬 호출
        MinigameManager.Instance.curMiniGame.GameEnd(rankings, response.EndTime);

        //미니게임 맵 삭제
        MinigameManager.Instance.boardCamera.SetActive(true);
        Destroy(MinigameManager.Instance.curMap.gameObject);
    }

    public void DartPannelSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartPannelSyncNotification;

        GameDartPanel panel = MinigameManager.Instance.GetMap<MapGameDart>().DartPanel;

        panel.moveDirection = ToVector3(response.Location);
        Debug.Log($"{panel.moveDirection} {response.Location}");
        if (MinigameManager.Instance.mySessonId.Equals(response.SessionId))
        {
        }
    }

    public void DartSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartSyncNotification;

        string sessionId = response.SessionId;
        int userIdx = GameManager.Instance.SessionDic[sessionId].Color;
        //Debug.Log($"{sessionId} {userIdx} {response.Angle}");

        DartPlayer dartUser = MinigameManager.Instance.GetMap<MapGameDart>().DartOrder[userIdx];

        dartUser.CurAim = ToVector3(response.Angle);
    }
}
