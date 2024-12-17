using System.Text;
using UnityEngine;

public class AreaNode : BaseNode, IPurchase
{
    private StringBuilder owner = new StringBuilder("");
    private Material baseMat;
    private int saleAmount = 10;
    private int index;
    public int ownerColor { get; private set; }

    //매수금은 판매액의 2배
    //벌금은 판매액의 반

    [SerializeField] MeshRenderer plane;

    string IPurchase.message => GetMessage();

    private void Start()
    {
        baseMat = plane.sharedMaterial;
        index = BoardManager.Instance.areaNodes.IndexOf(this);
    }

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

            else if (player.data.coin >= saleAmount)
                await UIManager.Show<PurchaseNodeUI>(purchase);
            else
                BoardManager.Instance.TurnEnd();
        }
        else
            Cancle();
    }
    private async void Penalty(BoardTokenData p)
    {
        IPurchase purchase = this;
        await UIManager.Show<PenaltyUI>(saleAmount >> 1,purchase,p);

        GamePacket packet = new();

        packet.TilePenaltyRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            Tile = index
        };

        SocketManager.Instance.OnSend(packet);

        //p.coin = Mathf.Max(p.coin - saleAmount, 0);
    }

    public void Purchase()
    {
        string id = GameManager.Instance.myInfo.SessionId;

        GamePacket packet = new();
        packet.PurchaseTileRequest = new()
        {
            SessionId = id,
            Tile = index
        };

        SocketManager.Instance.OnSend(packet);

        //if (owner.ToString() != "")
        //    saleAmount = (int)(saleAmount * 2f);

        Cancle();
    }

    public void Cancle()
    {
        BoardTokenHandler p = BoardManager.Instance.Curplayer;
        p.SetNode(nodes[0],true);
        p.GetDice(0);

        BoardManager.Instance.TurnEnd();
    }

    public void SetArea(string id,int sale)
    {
        if (!owner.Equals(""))
        {
            int c = GameManager.Instance.SessionDic[id].Color;
            UIManager.Get<BoardUI>().GetPlayerUI(c).Event((int)(saleAmount * 1.5f));
        }

        this.owner.Clear();
        this.owner.Append(id);
        int i = ownerColor = GameManager.Instance.SessionDic[id].Color;
        plane.material = BoardManager.Instance.materials[i];
        this.saleAmount = sale;
    }

    private string GetMessage()
    {
        if(owner.Equals(""))
            return $"{saleAmount}의 코인을 지불하여 해당 칸을 구매 할 수 있습니다.";
        else
            return $"{(int)(saleAmount * 1.5f)}의 코인을 지불하여 해당 칸을 인수 할 수 있습니다.";
    }

    public void ClearArea()
    {
        owner.Clear();
        plane.material = baseMat;
        saleAmount = 10;
    }
}
