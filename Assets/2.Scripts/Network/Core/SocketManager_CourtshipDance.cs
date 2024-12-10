using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    /* 401 */
    public void DanceMiniGameReadyNotification(GamePacket packet)
    {
        var response = packet.DanceMiniGameReadyNotification;
#pragma warning disable CS4014
        UIManager.Show<UIMinigameReady>(eGameType.GameCourtshipDance);
#pragma warning restore CS4014
        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(response);
        MinigameManager.Instance.boardCamera.SetActive(false);
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

        //ReadyUI 숨기기
        UIManager.Hide<UIMinigameReady>();
        //GameStart 함수 호출
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GameStart(response);
    }

    /* 405 : DanceTableCreateRequest
     * Send 위치 : GameCourtshipDance */

    /* 406 */
    public void DanceTableNotification(GamePacket packet)
    {
        var response = packet.DanceTableNotification;
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().SetCommandPoolDic(response.DancePools);
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().TrySetTask(true);

    }

    /* 407 : DanceKeyPressRequest
     * Send 위치 : CommandBoard */

    /* 408 */
    public void DanceKeyPressResponse(GamePacket packet)
    {
        var response = packet.DanceKeyPressResponse;
        if(response.Success)
        {
            UIManager.Get<UICourtshipDance>().boardDic[GameManager.Instance.myInfo.SessionId].OnEventInput(response.Correct);
        }
    }

    /* 409 */
    public void DanceKeyPressNotification(GamePacket packet)
    {
        var response = packet.DanceKeyPressNotification;
        UIManager.Get<UICourtshipDance>().boardDic[response.SessionId].OtherHandleInput(response.Correct, response.SessionId);
    }

    /* 410 */
    public void DanceGameOverNotification(GamePacket packet)
    {
        var response = packet.DanceGameOverNotification;      

        UIManager.Get<UICourtshipDance>().GameOver(response);
    }

    /* 411 */
    public void DanceTableCompleteRequest(GamePacket packet)
    {
        var response = packet.DanceTableCompleteRequest;
    }

    /* 412 : DanceTableCompleteRequest
     * Send 위치 : CommandBoard */


}
