using System.Collections.Generic;
using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public void DartMiniGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartMiniGameReadyNotification;
        //Debug.Log(response);

        UIManager.Hide<BoardUI>();
#pragma warning disable CS4014 
        UIManager.Show<UIMinigameReady>(eGameType.GameDart);
#pragma warning restore CS4014

        MinigameManager.Instance.SetMiniGame<GameDart>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
    }   
    
    public void DartGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameReadyNotification;
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    //필요한것 : Dart의 방향이 움직이는 동기화, 판도 서버에서 동기화 시키기

    public void DartMiniGameStartNotification(GamePacket gamePacket)
    {
        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameDart>().GameStart();
    }

    public async void DartGameThrowNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameThrowNotification;
        //Debug.Log(response.Result);

        int userIdx = GameManager.Instance.SessionDic[response.Result.SessionId].Color;

        var map = await MinigameManager.Instance.GetMap<MapGameDart>();
        map.DartOrder[userIdx].ApplyShoot(response.Result);
    }

    public void DartGameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartGameOverNotification;
        Debug.Log(response);

        List<(int Rank, string SessionId)> rankings = new();
        foreach(var r in response.Ranks)
        {
            rankings.Add((r.Rank_, r.SessionId));
        }

        //UI Minigame Result 판넬 호출
        MinigameManager.Instance.curMiniGame.GameEnd(rankings, response.EndTime);

        //미니게임 맵 삭제
        MinigameManager.Instance.boardCamera.SetActive(true);
        Destroy(MinigameManager.Instance.curMap.gameObject);
    }

    public async void DartPannelSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartPannelSyncNotification;

        //Debug.Log($"{response.Location}");

        //GameDartPanel panel = ClientTest.Instance.Panel;
        //panel.MoveEvent(response.Location);
        var map = await MinigameManager.Instance.GetMap<MapGameDart>();
        GameDartPanel panel = map.DartPanel;
        if (!MinigameManager.Instance.mySessonId.Equals(response.SessionId))
        {
            panel.MoveEvent(response.Location);
        }
    }

    public async void DartSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartSyncNotification;

        string sessionId = response.SessionId;
        int userIdx = GameManager.Instance.SessionDic[sessionId].Color;

        //Debug.Log($"{sessionId} {userIdx} {response.Angle}");

        if (!GameManager.Instance.myInfo.SessionId.Equals(sessionId))
        {
            var map = await MinigameManager.Instance.GetMap<MapGameDart>();
            DartPlayer dartUser = map.DartOrder[userIdx];
            dartUser.CurAim = ToVector3(response.Angle);
        }
    }

    public void DartPointNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DartPointNotification;
        Debug.Log(response);

        int userIdx = GameManager.Instance.SessionDic[response.SessionId].Color;
        MinigameManager.Instance.GetMiniGame<GameDart>().AddScore(userIdx, response.Point);
    }
}
