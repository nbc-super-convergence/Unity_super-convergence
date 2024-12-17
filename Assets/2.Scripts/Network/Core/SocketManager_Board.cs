using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    #region 보드

    public void BoardGoldSyncNotification(GamePacket packet)
    {
        var response = packet.BoardGoldSyncNotification;

        for (int i = 0; i < response.PlayersInfo.Count; i++)
        {
            var p = response.PlayersInfo[i];
            var data = BoardManager.Instance.GetToken(p.SessionId).data;
            data.coin = p.Gold;
        }

        UIManager.Get<BoardUI>().Refresh();
    }

    #region 주사위
    public void RollDiceResponse(GamePacket packet)
    {
        var response = packet.RollDiceResponse;

        if (response.Success)
        {
            var player = BoardManager.Instance.Curplayer;
            int dice = response.DiceResult;

            player.GetDice(dice);
            StartCoroutine(BoardManager.Instance.dice.SetDice(dice - 1));

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

        StartCoroutine(BoardManager.Instance.dice.SetDice(dice - 1));
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
        token?.ReceivePosition(pos, rotY);

        Debug.Log("MovePlayerBoardNotification");
    }

    #endregion

    #region 타일 구매
    public void PurchaseTileResponse(GamePacket packet)
    {
        var response = packet.PurchaseTileResponse;

        if (response.Success && response.IsPurchased)
        {
            var playerinfo = response.PlayerInfo;
            string id = playerinfo.SessionId;

            int i = response.Tile;
            int j = GameManager.Instance.SessionDic[id].Color;

            var data = BoardManager.Instance.GetToken(id).data;
            data.coin = playerinfo.Gold;

            BoardManager.Instance.areaNodes[i].SetArea(id,response.PurchaseGold);

            UIManager.Get<BoardUI>().GetPlayerUI(j).Event(-response.PurchaseGold);
            UIManager.Get<BoardUI>().Refresh();
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
        int j = GameManager.Instance.SessionDic[id].Color;

        var data = BoardManager.Instance.GetToken(id).data;
        data.coin = response.PlayerInfo.Gold;

        BoardManager.Instance.areaNodes[i].SetArea(id,response.PurchaseGold);

        UIManager.Get<BoardUI>().GetPlayerUI(j).Event(-response.PurchaseGold);
        UIManager.Get<BoardUI>().Refresh();
    }
    #endregion

    #region 트로피 구매
    //public void PurchaseTrophyResponse(GamePacket packet)
    //{
    //    var response = packet.PurchaseTrophyResponse;

    //    if(response.Success)
    //    {
    //        int i = response.NextTile;

    //        var playerinfo = response.PlayerInfo;
    //        string id = playerinfo.SessionId;

    //        var data = BoardManager.Instance.GetToken(id).data;
    //        data.keyAmount = playerinfo.Gold;
    //        data.trophyAmount = playerinfo.Trophy;

    //        var list = BoardManager.Instance.trophyNode;
    //        list[i].Toggle();
    //    }
    //    else
    //    {
    //        Debug.LogError($"FailCode : {response.FailCode.ToString()}");
    //    }
    //}
    
    //public void PurchaseTrophyNotification(GamePacket packet)
    //{
    //    var response = packet.PurchaseTrophyNotification;

    //    var playerinfos = response.PlayersInfo.ToList();

    //    for(int i = 0; i < playerinfos.Count; i++)
    //    {
    //        string id = playerinfos[i].SessionId;

    //        var data = BoardManager.Instance.GetToken(id).data;
    //        data.keyAmount = playerinfos[i].Gold;
    //        data.trophyAmount = playerinfos[i].Trophy;
    //    }

    //    int b = response.BeforeTile;
    //    int n = response.NextTile;

    //    BoardManager.Instance.trophyNode[b].Toggle();
    //    BoardManager.Instance.trophyNode[n].Toggle();
    //}

    #endregion

    #region 타일 패널티

    public void TilePenaltyResponse(GamePacket packet)
    {
        var response = packet.TilePenaltyResponse;

        if (response.Success)
        {
            var playerinfos = response.PlayersInfo.ToList();

            int penalty = int.MaxValue;

            for (int i = 0; i < playerinfos.Count; i++)
            {
                string id = playerinfos[i].SessionId;

                var data = BoardManager.Instance.GetToken(id).data;

                penalty = Mathf.Min(playerinfos[i].Gold - data.coin,penalty);
                data.coin = playerinfos[i].Gold;

                int j = GameManager.Instance.SessionDic[id].Color;

                if(penalty != 0)
                    UIManager.Get<BoardUI>().GetPlayerUI(j).Event(penalty);
                //data.trophyAmount = playerinfos[i].Trophy;
            }

            UIManager.Get<PenaltyUI>().SetTax(Mathf.Abs(penalty));
            UIManager.Get<BoardUI>().Refresh();
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

            int penalty = playerinfos[i].Gold - data.coin;
            data.coin = playerinfos[i].Gold;

            int j = GameManager.Instance.SessionDic[id].Color;

            if (penalty != 0)
                UIManager.Get<BoardUI>().GetPlayerUI(j).Event(penalty);
            //data.trophyAmount = playerinfos[i].Trophy;
        }

        UIManager.Get<BoardUI>().Refresh();
    }

    #endregion

    #region 턴 종료

    public void TurnEndNotification(GamePacket packet)
    {
        BoardManager.Instance.NextTurn();
        Debug.Log("TurnEndNotification");
    }

    #endregion

    #endregion

    #region 게임종료

    public async void GameEndNotification(GamePacket packet)
    {
        var response = packet.GameEndNotification;
        UIManager.Hide<BoardUI>();

        for (int i = 0; i < response.Rank.Count; i++)
        {
            var p = response.Rank[i];
            var data = BoardManager.Instance.GetToken(p.SessionId).data;
            data.coin = p.Gold;
        }

        await UIManager.Show<BoardResultUI>();
        //게임종료 필요
        //BoardManager.Instance.GameOver();
    }

    private Coroutine defenseCoroutine;
    public void BackToTheRoomResponse(GamePacket packet)
    {
        var response = packet.BackToTheRoomResponse;

        if (response.Success)
        {
            defenseCoroutine ??= StartCoroutine(ReturnStartScene(response));
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void BackToTheRoomNotification(GamePacket packet)
    {
        var response = packet.BackToTheRoomNotification;
    }

    private IEnumerator ReturnStartScene(S2C_BackToTheRoomResponse response)
    {
        SceneManager.LoadScene(0);

        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 0);

        yield return UIManager.Show<UIRoom>(response.Room);

        defenseCoroutine = null;
    }

    #endregion

}
