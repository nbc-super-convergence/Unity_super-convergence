using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    #region ����

    #region �ֻ���
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

    #region ���� �÷��̾� �̵�
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

    #region Ÿ�� ����
    public void PurchaseTileResponse(GamePacket packet)
    {
        var response = packet.PurchaseTileResponse;

        if (response.Success)
        {
            int i = response.Tile;

            var player = BoardManager.Instance.Curplayer.data;
            string id = player.userInfo.SessionId;

            BoardManager.Instance.areaNodes[i].SetArea(id);
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void purchaseTileNotification(GamePacket packet)
    {
        var response = packet.PurchaseTileNotification;

        string id = response.SessionId;

        int i = response.Tile;
        var player = BoardManager.Instance.GetToken(id);

        BoardManager.Instance.areaNodes[i].SetArea(id);
    }
    #endregion

    #region Ʈ���� ����
    public void PurchaseTrophyResponse(GamePacket packet)
    {
        var response = packet.PurchaseTrophyResponse;

        if(response.Success)
        {
            int i = response.NextTile;

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

        string id = response.SessionId;
        var player = BoardManager.Instance.GetToken(id).data;
        player.trophyAmount += 1;

        int i = response.BeforeTile;
        int j = response.NextTile;

        BoardManager.Instance.trophyNode[i].Toggle();
        BoardManager.Instance.trophyNode[j].Toggle();
    }

    #endregion

    #region Ÿ�� �г�Ƽ

    public void TilePenaltyRequest(GamePacket packet)
    {
        var response = packet.TilePenaltyResponse;

        if (response.Success)
        {

        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void TilePenaltyResponse(GamePacket packet)
    {
        var response = packet.TilePenaltyNotification;

    }

    #endregion


    #endregion

    #region ��������

    public void GameEndNotification(GamePacket packet)
    {
        var response = packet.GameEndNotification;

        //�������� �ʿ�
        //BoardManager.Instance
    }

    #endregion
}
