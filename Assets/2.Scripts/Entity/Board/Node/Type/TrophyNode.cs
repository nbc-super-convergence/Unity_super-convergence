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
        if(isTrophy)
        {
            var player = BoardManager.Instance.Curplayer;
            int index = BoardManager.Instance.curPlayerIndex;
            IPurchase purchase = this;

            var ui = await UIManager.Show<PurchaseNodeUI>(purchase,index);
        }
    }

    protected override bool IsStopCondition()
    {
        return isTrophy;
    }

    public void Purchase(int index)
    {
        ////트로피 + 1
        //BoardManager.Instance.Curplayer.data.trophyAmount += 1;

        //GamePacket packet = new();

        ////packet. = new()
        ////{

        ////};

        //SocketManager.Instance.OnSend(packet);

        Cancle();
    }

    public void Cancle()
    {
        PlayerTokenHandler p = BoardManager.Instance.Curplayer;
        p.SetNode(nodes[0],true);
        p.GetDice(0);

        BoardManager.Instance.TurnEnd();
    }
}
