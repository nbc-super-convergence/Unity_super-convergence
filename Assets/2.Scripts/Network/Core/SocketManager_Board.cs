using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    #region 보드

    public void RollDiceResponse(GamePacket packet)
    {
        var response = packet.RollDiceResponse;

        if (response.Success)
        {
            var player = BoardManager.Instance.Curplayer;
            int dice = response.DiceResult;

            player.GetDice(dice);
            Debug.Log("RollDiceResponse");
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void RollDiceNotification(GamePacket packet)
    {
        var response = packet.RollDiceNotification;

        var player = BoardManager.Instance.Curplayer;
        int dice = response.DiceResult;

        Debug.Log("RollDiceNotification");
    }

    public void MovePlayerBoardResponse(GamePacket packet)
    {
        var response = packet.MovePlayerBoardResponse;

        if (response.Success)
        {
            Debug.Log("MovePlayerBoardResponse");
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void MovePlayerBoardNotification(GamePacket packet)
    {
        var response = packet.MovePlayerBoardNotification;

        Vector3 pos = ToVector3(response.TargetPoint);
        float rotY = response.Rotation;

        var token = BoardManager.Instance.GetToken(response.SessionId);
        token.ReceivePosition(pos, rotY);

        Debug.Log("MovePlayerBoardNotification");
    }

    public void PurchaseTileResponse(GamePacket packet)
    {
        var response = packet.PurchaseTileResponse;

        if (response.Success)
        {
            int i = response.Tile;
            var player = BoardManager.Instance.Curplayer;
            int p = BoardManager.Instance.Curplayer.data.userInfo.Order;
            int c = BoardManager.Instance.Curplayer.data.userInfo.Color;

            BoardManager.Instance.areaNodes[i].SetArea(p,c);
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void purchaseTileNotification(GamePacket packet)
    {
        var response = packet.PurchaseTileNotification;

        var player = BoardManager.Instance.GetToken(response.SessionId);
        int i = response.Tile;
        int p = player.data.userInfo.Order;
        int c = player.data.userInfo.Color;

        BoardManager.Instance.areaNodes[i].SetArea(p, c);
    }

    #endregion

    #region 게임종료

    public void GameEndNotification(GamePacket packet)
    {
        var response = packet.GameEndNotification;

        //게임종료 필요
        //BoardManager.Instance
    }

    #endregion
}
