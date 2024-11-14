
using UnityEngine;

public class ActionArea : IPurchase
{
    private int playerIndex = -1;
    [SerializeField] MeshRenderer plane;

    public ActionArea(MeshRenderer plane)
    {
        this.plane = plane;
    }

    public async void Action()
    {
        var m = MapControl.Instance;
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
        //p.hp -= 0;
    }

    public void Purchase(int index)
    {
        playerIndex = index;
        plane.material = MapControl.Instance.materials[index];
    }
}
