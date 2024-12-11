using System.Text;
using UnityEngine;

public class AreaNode : BaseNode, IPurchase
{
    private StringBuilder owner = new StringBuilder("");
    private int saleAmount = 10;
    //매수금은 판매액의 2배
    //벌금은 판매액의 반

    [SerializeField] MeshRenderer plane;

    string IPurchase.message => $"{saleAmount}의 코인을 지불하여 해당 칸을 구매 할 수 있습니다.";


    public async override void Action()
    {
        int c = BoardManager.Instance.Curplayer.queue.Count;
        int d = BoardManager.Instance.Curplayer.dice;

        if (c > 1 || d > 0)
        {
            base.Action();
            return;
        }

        //var m = BoardManager.Instance;
        //int index = m.playerTokenHandlers.IndexOf(p);

        var player = BoardManager.Instance.Curplayer;
        int index = BoardManager.Instance.curPlayerIndex;
        IPurchase purchase = this;

        //if (playerIndex == -1)
        //{
        //    var ui = await UIManager.Show<PurchaseNodeUI>(purchase, index);
        //    return;
        //}
        string o = owner.ToString();
        string id = GameManager.Instance.myInfo.SessionId;

        if (o != id)
        {
            if (o != "") Penalty(player.data);

            if (player.data.keyAmount >= saleAmount)
                await UIManager.Show<PurchaseNodeUI>(purchase);
            else
                BoardManager.Instance.TurnEnd();
        }
        else
            Cancle();
    }
    private void Penalty(BoardTokenData p)
    {
        GamePacket packet = new();

        packet.TilePenaltyRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            Tile = BoardManager.Instance.areaNodes.IndexOf(this)
        };

        SocketManager.Instance.OnSend(packet);
    }

    public void Purchase()
    {
        string id = GameManager.Instance.myInfo.SessionId;

        GamePacket packet = new();
        packet.PurchaseTileRequest = new()
        {
            SessionId = id,
            Tile = BoardManager.Instance.areaNodes.IndexOf(this)
        };

        SocketManager.Instance.OnSend(packet);

        //if (owner.ToString() != "")
        //    saleAmount = (int)(saleAmount * 2f);

        SetArea(id);
        Cancle();
    }

    public void Cancle()
    {
        BoardTokenHandler p = BoardManager.Instance.Curplayer;
        p.SetNode(nodes[0],true);
        p.GetDice(0);

        BoardManager.Instance.TurnEnd();
    }

    public void SetArea(string id)
    {
        this.owner.Clear();
        this.owner.Append(id);
        int i = GameManager.Instance.SessionDic[id].Color;

        plane.material = BoardManager.Instance.materials[i];
    }
}
