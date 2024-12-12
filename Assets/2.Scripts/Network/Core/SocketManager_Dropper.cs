using System.Collections.Generic;
using System.Linq;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //301 
    public void DropMiniGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropMiniGameReadyNotification;

        //ReadyPanel 띄우기.
        UIManager.Hide<BoardUI>();
#pragma warning disable CS4014
        UIManager.Show<UIMinigameReady>(eGameType.GameDropper);
#pragma warning restore CS4014

        //데이터 설정, 맵 설정, BGM 설정
        MinigameManager.Instance.SetMiniGame<GameDropper>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
    }

    //302 dropGameReadyRequest

    //303
    public void DropGameReadyNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropGameReadyNotification;

        //ReadyUI와 연계
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    //304
    public void DropMiniGameStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropMiniGameStartNotification;

        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameDropper>().GameStart(response.StartTime);
    }

    //305 DropPlayerSyncRequest

    //306
    public void DropPlayerSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropPlayerSyncNotification;

        MinigameManager.Instance.GetMiniGame<GameDropper>()
            .ReceiveMove(response.SessionId, response.Slot, response.Rotation, response.State);
    }

    //307
    public void DropPlayerDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropPlayerDeathNotification;

        //플레이어 사망 이벤트
        MinigameManager.Instance.GetMiniGame<GameDropper>()
            .PlayerDeath(response.SessionId);
    }

    //308
    public void DropLevelStartNotification(GamePacket gamePacket)
    {
        
    }

    //309
    public void DropLevelEndNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropLevelEndNotification;
        int[] holes = response.Holes.ToArray();

        //글씨 변경, 2.5초 input 잠금.
        StartCoroutine(UIManager.Get<UIMinigameDropper>().MovableTime());

        //1초 후 구멍뚫기.
        StartCoroutine(MinigameManager.Instance.GetMap<MapGameDropper>().NextLevelEvent(holes));
        
        
    }

    //310
    public void DropGameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DropGameOverNotification;

        /*필요 데이터 파싱*/
        Dictionary<string, int> rankings = new();
        foreach (var r in response.Ranks)
        {
            rankings.Add(r.SessionId, r.Rank_);
        }

        //UI Minigame Result 판넬 호출
        MinigameManager.Instance.curMiniGame.GameEnd(rankings, response.EndTime);

        //미니게임 맵 삭제
        MinigameManager.Instance.boardCamera.SetActive(true);
        Destroy(MinigameManager.Instance.curMap.gameObject);
    }
}