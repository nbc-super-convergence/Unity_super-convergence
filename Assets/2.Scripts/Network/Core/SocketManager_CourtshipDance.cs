
public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    /* 401 */
    public void DanceMiniGameReadyNotification(GamePacket packet)
    {
        var response = packet.DanceMiniGameReadyNotification;
        UIManager.Hide<BoardUI>();
        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
#pragma warning disable CS4014
        UIManager.Show<UIMinigameReady>(eGameType.GameCourtshipDance);
#pragma warning restore CS4014
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
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().SetTeamPoolDic(response.DancePools);
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().TrySetTask(response.DancePools != null);
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
        MinigameManager.Instance.GetMyToken().InputHandler.DisableSimpleInput();
        UIManager.Get<UICourtshipDance>().GameOver(response);
    }

    /* 411 */
    public void DanceCloseSocketNotification(GamePacket packet)
    {
        var response = packet.DanceCloseSocketNotification;
        MinigameManager.Instance.GetMap<MapGameCourtshipDance>().DanceCloseSocketNotification(
            response.DisconnectedSessionId, response.ReplacementSessionId);
    }

    /* 412 : DanceTableCompleteRequest
     * Send 위치 : CommandBoard */


}
