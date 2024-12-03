
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    }

    #region 보드 플레이어 이동
    public void MovePlayerBoardResponse(GamePacket packet)
    {
        var response = packet.MovePlayerBoardResponse;

        if (response.Success)
        {

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
        int index = response.PlayerId;

        Vector3 pos = ToVector3(response.TargetPoint);
        var players = BoardManager.Instance.playerTokenHandlers;
        players[index].ReceivePosition(pos,response.Rotation);
    }
    #endregion

    #region 노드 구매
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

        int id = response.PlayerId;
        //int index = response.Tile; //Vector -> int로 변경 필요
        //BoardManager.Instance.PurChaseNode(indexer,id);
    }
    #endregion

    #region 트로피 구매

    public void PurchaseTrophyResponse(GamePacket packet)
    {
        var response = packet.PurchaseTrophyResponse;

        if(response.Success)
        {
            string id = response.PlayerInfo.SessionId;
            var user = GameManager.Instance.SessionDic[id];
            int i = user.Order;
            BoardManager.Instance.playerTokenHandlers[i].data.trophyAmount += 1;

            int j = response.NextTile;
            BoardManager.Instance.trophyNode[j].Toggle();
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void PurchaseTrophyNotification(GamePacket packet)
    {
        var response = packet.PurchaseTrophyNotification;

        int p = response.BeforeTile;
        int n = response.NextTile;

        var list = BoardManager.Instance.trophyNode;

        list[p].Toggle();
        list[n].Toggle();
    }

    #endregion

    #region 세금 납부

    public void TilePenaltyResponse(GamePacket packet)
    {
        var response = packet.TilePenaltyResponse;

        if(response.Success)
        {

        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void TilePenaltyNotification(GamePacket packet)
    {
        var response = packet.TilePenaltyNotification;

        int i = response.Tile;
    }

    #endregion

    #endregion

    #region 게임종료

    public void GameEndNotification(GamePacket packet)
    {
        var response = packet.GameEndNotification;

        //게임종료 필요
        //BoardManager.Instance
    }

    #endregion

    #region 방으로 돌아가기

    public void BackToTheRoomResponse(GamePacket packet)
    {
        var response = packet.BackToTheRoomResponse;

        if (response.Success)
        {
            //어떻게 써야할지 물어봐야함
            //response.Room

        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    #endregion
}
