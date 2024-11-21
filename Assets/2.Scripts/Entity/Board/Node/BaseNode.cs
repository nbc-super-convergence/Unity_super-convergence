using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour, IBoardNode,IAction
{
    public List<Transform> nodes;
    public BoardArrow arrowPrefab;

    private List<GameObject> arrows;

    private void Awake()
    {
        nodes = new();
        if (nodes.Count > 1)
        {
            CreateArrow();
            ActiveArrow(false);
        }
    }

    public virtual bool TryGetNode(out Transform node)
    {
        node = transform;

        if (nodes.Count > 1)
        {
            StartCoroutine(ArrivePlayer());
            return false;
        }
        else
            node = nodes[0];

        return true;
    }

    protected IEnumerator ArrivePlayer()
    {
        Transform p = BoardManager.Instance.Curplayer.transform;

        while(true)
        {
            if (transform.position.Equals(p.position))
                break;

            yield return null;
        }

        Action();
    }

    private void ActiveArrow(bool active)
    {
        foreach(var g in arrows)
            g.SetActive(active);
    }

    private async void CreateArrow()
    {
        arrows = new();

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 pos = (nodes[i].transform.position - transform.position) / 2;
            float angle = Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg;
            float dX = arrowPrefab.transform.localEulerAngles.x;

            BoardArrow a = Instantiate(arrowPrefab, transform.position + pos, Quaternion.Euler(dX, -angle + dX, 0));
            arrows.Add(a.gameObject);
            a.SetNode(nodes[i]);

            a.OnEvent += () => ActiveArrow(false);
        }
    }

    public virtual void Action()
    {
        ActiveArrow(true);
    }
}
