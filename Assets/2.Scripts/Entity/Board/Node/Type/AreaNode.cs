using UnityEngine;

public class AreaNode : BaseNode, IPurchase
{
    private int playerIndex = -1;
    [SerializeField] MeshRenderer plane;
    int price;

    string IPurchase.message => $"{price}�� ���踦 �����Ͽ� �ش� ĭ�� ���� �� �� �ֽ��ϴ�.";


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

        if (playerIndex != index)
        {
            if (playerIndex != -1) Damage(player.data);
            var ui = await UIManager.Show<PurchaseNodeUI>(purchase, index);
        }
    }
    private void Damage(PlayerTokenData p)
    {
        //�ӽ� �ּ�
        //p.hp -= 0;
    }

    public void Purchase(int index)
    {
        playerIndex = index;
        plane.material = BoardManager.Instance.materials[index];
    }

    public void Cancle()
    {
        PlayerTokenHandler p = BoardManager.Instance.Curplayer;
        p.SetNode(nodes[0],true);
        p.GetDice(0);
    }
}
