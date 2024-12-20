using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour, IBoardNode,IAction
{
    public List<Transform> nodes;
    //public BoardArrow arrowPrefab;

    private List<GameObject> arrows;


    public Transform lineUp;

    private List<Transform> players = new();

    private void Awake()
    {
        //GameObject g = await ResourceManager.Instance.LoadAsset<GameObject>("arrow", eAddressableType.Prefab);
        players = new();

        if (nodes.Count > 1)
            StartCoroutine(Test());
            //CreateArrow();
    }

    private IEnumerator Test()
    {
        yield return new WaitUntil(() => ResourceManager.Instance.isInitialized);
        CreateArrow();
    }

    public virtual bool TryGetNode(out Transform node)
    {
        node = transform;

        if (IsStopCondition())
        {
            //Debug.Log(1);
            //StartCoroutine(ArrivePlayer());
            return false;
        }
        else
            node = nodes[0];

        return true;
    }

    //protected IEnumerator ArrivePlayer()
    //{
    //    Transform p = BoardManager.Instance.Curplayer.transform;

    //    while(true)
    //    {
    //        if (transform.position.Equals(p.position))
    //            break;

    //        yield return null;
    //    }

    //    Action();
    //}

    private void ActiveArrow(bool active)
    {
        foreach(var g in arrows)
            g.SetActive(active);
    }

    private async void CreateArrow()
    {
        if (!ResourceManager.Instance.isInit) return;

        arrows = new();
        GameObject arrowPrefab = await ResourceManager.Instance.LoadAsset<GameObject>("arrow", eAddressableType.Prefab);

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 pos = (nodes[i].transform.position - transform.position) / 2;
            float angle = Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg;
            float dX = arrowPrefab.transform.localEulerAngles.x;

            GameObject g = Instantiate(arrowPrefab, transform.position + pos, Quaternion.Euler(dX, -angle + dX, 0));
            arrows.Add(g);

            BoardArrow a = g.GetComponent<BoardArrow>();
            a.SetNode(nodes[i]);
            a.OnEvent += () => ActiveArrow(false);
        }

        ActiveArrow(false);
    }

    public virtual void Action()
    {
        if(nodes.Count > 1) ActiveArrow(true);
    }

    protected virtual bool IsStopCondition()
    {
        return nodes.Count > 1;
    }

    public void LineUp()
    {
        //Vector3 pos = lineUp.position;

        //for (int i = 0; i < players.Count; i++)
        //    players[i].position = new Vector3(pos.x, 0, pos.z);
    }

    public List<Transform> GetList()
    {
        return players;
    }
}
