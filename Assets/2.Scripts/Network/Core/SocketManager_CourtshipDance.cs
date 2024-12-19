using System.Collections;
using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    /* 401 */
    public void DanceMiniGameReadyNotification(GamePacket packet)
    {
        var response = packet.DanceMiniGameReadyNotification;
        UIManager.SceneChangeTask = new();
        UIManager.Instance.LoadingScreen.OnLoadingEvent(UIManager.SceneChangeTask);
        StartCoroutine(CouroutineGameReady());

        IEnumerator CouroutineGameReady()
        {
            UIManager.Hide<BoardUI>();
            MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(response);
            var game = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>();
            yield return new WaitUntil(() => game.isInitialized);
            
            MinigameManager.Instance.boardCamera.SetActive(false);
#pragma warning disable CS4014
            UIManager.Show<UIMinigameReady>(eGameType.GameCourtshipDance);
            yield return new WaitForSeconds(1f);
#pragma warning restore CS4014
            UIManager.SceneChangeTask.SetResult(true);
        }
    }

    /* 402 : DanceReadyRequest
     * Send 위치 : UIMinigameReady */

    /* 403 */
    public void DanceReadyNotification(GamePacket packet)
    {
        var response = packet.DanceReadyNotification;
        //ReadyUI와 연계
        UIManager.Get<UIMinigameReady>().SetReady(response.SessionId);
    }

    /* 404 */
    public void DanceStartNotification(GamePacket packet)
    {
        var response = packet.DanceStartNotification;
        long startTime = response.StartTime;
        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GameStart(startTime);
    }

    /* 405 : DanceTableCreateRequest
     * Send 위치 : GameCourtshipDance */

    /* 406  : 404보다 먼저 옴*/
    public void DanceTableNotification(GamePacket packet)
    {
        var response = packet.DanceTableNotification;
        var game = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>();
        game.SetTeamPoolDic(response.DancePools);
    }

    /* 407 : DanceKeyPressRequest
     * Send 위치 : CommandBoard */

    /* 408 */
    public void DanceKeyPressResponse(GamePacket packet)
    {
        var response = packet.DanceKeyPressResponse;
        if(response.Success)
        {
            UIManager.Get<UICourtshipDance>().myBoard.MyInputResponse(response.Correct, response.State);
        }
    }

    /* 409 */
    public void DanceKeyPressNotification(GamePacket packet)
    {        
        var response = packet.DanceKeyPressNotification;
        var ui = UIManager.Get<UICourtshipDance>();
        if (response.TeamNumber != ui.myBoard.TeamNumber)
        {
            ui.boardDic[response.TeamNumber].OtherBoardNoti(response.TeamNumber, response.Correct, response.State);
        }
        else
        {
            ui.myBoard.MyTeamNotification(response.Correct, response.State);
        }
    }

    /* 410 */
    // 타이머가 다 끝나거나 한 유저가 댄스풀을 다 마쳤을 때.
    public void DanceGameOverNotification(GamePacket packet)
    {
        var response = packet.DanceGameOverNotification;
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().isGameOver = true;
        MinigameManager.Instance.GetMyToken().InputHandler.DisableSimpleInput();
        UIManager.Get<UICourtshipDance>().GameOver(response);
    }

    /* 411 */
    public async void DanceCloseSocketNotification(GamePacket packet)
    {
        if (MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().isGameOver == true) return;
        var response = packet.DanceCloseSocketNotification;

        MapGameCourtshipDance map = await MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
        map.DanceCloseSocketNotification(
            response.DisconnectedSessionId, response.ReplacementSessionId);
    }

    /* 412 : DanceTableCompleteRequest
     * Send 위치 : CommandBoard */
}
