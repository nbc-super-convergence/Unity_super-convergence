using UnityEngine;
using UnityEngine.UIElements;

public class AreaNode : BaseNode, IPurchase
{
    private int owner = -1;
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

        if (owner != index)
        {
            if (owner != -1) Penalty(player.data);
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
        //세션id 숙지 필요
        //playerIndex = index;
        //plane.material = BoardManager.Instance.materials[index];
        //int i = BoardManager.Instance.areaNodes.IndexOf(this);

        GamePacket packet = new();
        packet.PurchaseTileRequest = new()
        {
            //추후 변경되면 수정
            SessionId = GameManager.Instance.myInfo.SessionId,
            //Tile = BoardManager.Instance.areaNodes.IndexOf(this)
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

    public void SetArea(int index)
    {
        //playerIndex = index;
        plane.material = BoardManager.Instance.materials[index];
    }
}
