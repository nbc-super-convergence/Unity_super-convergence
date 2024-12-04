using System.Linq;
using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    #region 보드

    #region 주사위
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

    #endregion

    #region 보드 플레이어 이동
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

    #endregion

    #region 타일 구매
    public void PurchaseTileResponse(GamePacket packet)
    {
        var response = packet.PurchaseTileResponse;

        if (response.Success)
        {
            int i = response.Tile;

            var playerinfo = response.PlayerInfo;
            string id = playerinfo.SessionId;

            var data = BoardManager.Instance.GetToken(id).data;
            data.keyAmount = playerinfo.Gold;
            data.trophyAmount = playerinfo.Trophy;

            BoardManager.Instance.areaNodes[i].SetArea(id);
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void PurchaseTileNotification(GamePacket packet)
    {
        var response = packet.PurchaseTileNotification;

        string id = response.SessionId;

        int i = response.Tile;
        var player = BoardManager.Instance.GetToken(id);

        BoardManager.Instance.areaNodes[i].SetArea(id);
    }
    #endregion

    #region 트로피 구매
    public void PurchaseTrophyResponse(GamePacket packet)
    {
        var response = packet.PurchaseTrophyResponse;

        if(response.Success)
        {
            int i = response.NextTile;

            var playerinfo = response.PlayerInfo;
            string id = playerinfo.SessionId;

            var data = BoardManager.Instance.GetToken(id).data;
            data.keyAmount = playerinfo.Gold;
            data.trophyAmount = playerinfo.Trophy;

            var list = BoardManager.Instance.trophyNode;
            list[i].Toggle();
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }
    
    public void PurchaseTrophyNotification(GamePacket packet)
    {
        var response = packet.PurchaseTrophyNotification;

        var playerinfos = response.PlayersInfo.ToList();

        for(int i = 0; i < playerinfos.Count; i++)
        {
            string id = playerinfos[i].SessionId;

            var data = BoardManager.Instance.GetToken(id).data;
            data.keyAmount = playerinfos[i].Gold;
            data.trophyAmount = playerinfos[i].Trophy;
        }

        int b = response.BeforeTile;
        int n = response.NextTile;

        BoardManager.Instance.trophyNode[b].Toggle();
        BoardManager.Instance.trophyNode[n].Toggle();
    }

    #endregion

    #region 타일 패널티

    public void TilePenaltyResponse(GamePacket packet)
    {
        var response = packet.TilePenaltyResponse;

        if (response.Success)
        {
            var playerinfos = response.PlayersInfo.ToList();

            for(int i = 0; i < playerinfos.Count; i++)
            {
                string id = playerinfos[i].SessionId;

                var data = BoardManager.Instance.GetToken(id).data;
                data.keyAmount = playerinfos[i].Gold;
                data.trophyAmount = playerinfos[i].Trophy;
            }
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void TilePenaltyNotification(GamePacket packet)
    {
        var response = packet.TilePenaltyNotification;

        var playerinfos = response.PlayersInfo.ToList();

        for (int i = 0; i < playerinfos.Count; i++)
        {
            string id = playerinfos[i].SessionId;

            var data = BoardManager.Instance.GetToken(id).data;
            data.keyAmount = playerinfos[i].Gold;
            data.trophyAmount = playerinfos[i].Trophy;
        }
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
}
