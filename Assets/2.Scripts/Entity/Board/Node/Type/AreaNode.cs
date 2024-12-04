using System.Text;
using UnityEngine;

public class AreaNode : BaseNode, IPurchase
{
    private StringBuilder owner = new StringBuilder("");
    private int saleAmount = 10; //매수금은 판매액의 2배


    [SerializeField] MeshRenderer plane;
    int price;

    string IPurchase.message => $"{price}의 열쇠를 지불하여 해당 칸을 구매 할 수 있습니다.";


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

            var ui = await UIManager.Show<PurchaseNodeUI>(purchase, index);
        }
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

    public void Purchase(int index)
    {
        GamePacket packet = new();
        packet.PurchaseTileRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            Tile = BoardManager.Instance.areaNodes.IndexOf(this)
        };

        SocketManager.Instance.OnSend(packet);

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
        //this.owner = owner;
        this.owner.Clear();
        this.owner.Append(id);
        int i = GameManager.Instance.SessionDic[id].Color;

        plane.material = BoardManager.Instance.materials[i];
    }
}
