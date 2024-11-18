
using UnityEngine;

public class AreaNode : BaseNode,IPurchase
{
    private int playerIndex = -1;
    [SerializeField] MeshRenderer plane;

    public async override void Action()
    {
        var m = BoardManager.Instance;
        var p = m.Curplayer;
        int index = m.playerTokenHandlers.IndexOf(p);

        if (playerIndex == -1)
        {
            var ui = await UIManager.Show<PurchaseNodeUI>(this, index);
            return;
        }

        if (playerIndex != index)
        {
            Damage(p.data);
            var ui = await UIManager.Show<PurchaseNodeUI>(this, index);
        }
    }
    private void Damage(PlayerTokenData p)
    {
        //임시 주석
        //p.hp -= 0;
    }

    public void Purchase(int index)
    {
        playerIndex = index;
        plane.material = BoardManager.Instance.materials[index];
    }
}
