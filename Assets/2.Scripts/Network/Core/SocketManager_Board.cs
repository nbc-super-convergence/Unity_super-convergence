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
        //int index = GameManager.Instance.SessionDic //response.PlayerId;

        //플레이어id 활용방안 숙지 필요
        //int index = response.PlayerId;

        Vector3 pos = ToVector3(response.TargetPoint);
        var players = BoardManager.Instance.playerTokenHandlers;
        float rotY = response.Rotation;

        string id = response.SessionId;
        var user = GameManager.Instance.SessionDic[id];
        int i = user.Order;
        BoardManager.Instance.playerTokenHandlers[i].ReceivePosition(pos, rotY);

        Debug.Log("MovePlayerBoardNotification");
    }

    public void PurchaseTileResponse(GamePacket packet)
    {
        var response = packet.PurchaseTileResponse;

        if (response.Success)
        {
            //플레이어id 숙지 필요

            //int index = response.Tile; //Vector -> int로 변경 필요
            //int p = BoardManager.Instance.curPlayerIndex;
            //BoardManager.Instance.areaNodes[0].SetArea(p);
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void purchaseTileNotification(GamePacket packet)
    {
        var response = packet.PurchaseTileNotification;

        //int id = response.PlayerId;
        //int index = response.Tile; //Vector -> int로 변경 필요
        //BoardManager.Instance.PurChaseNode(indexer,id);
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
