using Unity.VisualScripting;
using UnityEngine;

public class TrophyNode : BaseNode,IToggle,IPurchase
{
    private bool isTrophy;
    private int price;

    public string message => $"{price}의 열쇠를 지불하여 트로피를 구매 할 수 있습니다.";


    private void Start()
    {
        BoardManager.Instance.trophyNode.Add(this);
    }

    public void Toggle()
    {
        isTrophy = !isTrophy;
    }

    //public override bool TryGetNode(out Transform node)
    //{
    //    node = transform;

    //    if (nodes.Count > 1 || isTrophy)
    //    {
    //        StartCoroutine(ArrivePlayer());
    //        return false;
    //    }
    //    else
    //        node = nodes[0];

    //    return true;
    //}

    public async override void Action()
    {
        var player = BoardManager.Instance.Curplayer;

        if(isTrophy)
        {
            int index = BoardManager.Instance.curPlayerIndex;
            IPurchase purchase = this;

            var ui = await UIManager.Show<PurchaseNodeUI>(purchase);
        }
        else if(player.IsTurnEnd())
        {
            BoardManager.Instance.TurnEnd();
        }
    }

    protected override bool IsStopCondition()
    {
        return isTrophy;
    }

    public void Purchase()
    {
        GamePacket packet = new();

        packet.PurchaseTrophyRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            Tile = BoardManager.Instance.trophyNode.IndexOf(this)
        };

        SocketManager.Instance.OnSend(packet);

        Toggle();
        Cancle();
    }

    public void Cancle()
    {
        BoardTokenHandler p = BoardManager.Instance.Curplayer;
        p.SetNode(nodes[0],true);
        p.GetDice(0);

        BoardManager.Instance.TurnEnd();
    }
}
