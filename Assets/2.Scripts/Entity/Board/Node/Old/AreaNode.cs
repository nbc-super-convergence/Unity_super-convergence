using UnityEngine;

//public class AreaNode : BaseNode,IAreaNode
//{
//    private int playerIndex = -1;
//    [SerializeField] MeshRenderer plane;

//    public void Action()
//    {
//        StartCoroutine(ArrivePlayer(AreaAction));
//    }

//    private async void AreaAction()
//    {
//        var m = MapControl.Instance;
//        var p = m.Curplayer;
//        int index = m.playerTokenHandlers.IndexOf(p);

//        if (playerIndex == -1)
//        {
//            var ui = await UIManager.Show<PurchaseNodeUI>(this, index);
//            return;
//        }

//        if (playerIndex != index)
//        {
//            Damage(p.data);
//            var ui = await UIManager.Show<PurchaseNodeUI>();
//        }
//    }

//    private void Damage(PlayerTokenData p)
//    {
//        p.hp -= 0;
//    }

//    public void Purchase(int index)
//    {
//        playerIndex = index;
//        plane.material = MapControl.Instance.materials[index];
//    }

//    //public async void Action(PlayerTokenHandler player)
//    //{
//    //    int index = MapControl.Instance.playerTokenHandlers.IndexOf(player);

//    //    if(playerIndex == -1) 
//    //    {
//    //        await UIManager.Show<PurchaseTile>(this, index);
//    //    }
//    //    else 
//    //    {
//    //        Damage(player.data);
//    //        //UIManager.Show<> 
//    //    }
//    //}

//    //public void Purchase(PlayerTokenHandler player)
//    //{
//    //    GetComponent<Renderer>().material = player.GetComponent<MeshRenderer>().material;
//    //}

//    //private void Damage(PlayerTokenData data)
//    //{
//    //    //data.hp -= 0;
//    //}
//}
