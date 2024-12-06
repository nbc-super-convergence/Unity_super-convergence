using System.Collections.Generic;
using UnityEngine;

public class BoardUI : UIBase
{
    public Transform parent;
    private List<Transform> players;

    public override async void Opened(object[] param)
    {
        base.Opened(param);

        //GameObject prefab = await ResourceManager.Instance.LoadAsset<GameObject>("boardtokenui", eAddressableType.Prefab);

        //var list = BoardManager.Instance.playerTokenHandlers;

        //for (int i = 0; i < list.Count; i++)
        //{
        //    BoardTokenUI ui = Instantiate(prefab,parent).GetComponent<BoardTokenUI>();

        //    var data = list[i].data;
        //    ui.SetPlayer(data);
        //}
    }
}
