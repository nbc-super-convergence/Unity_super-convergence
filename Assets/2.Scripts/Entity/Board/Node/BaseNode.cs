using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour, IBoardNode
{
    public List<Transform> nodes => nextNode;
    public List<Transform> nextNode = new();
    public BoardArrow arrowPrefab;

    private List<GameObject> arrows;

    private void Awake()
    {
        if (nextNode.Count > 1)
        {
            CreateArrow();
            ActiveArrow(false);
        }
    }

    public bool TryRunNextNode(out Transform node)
    {
        node = transform;

        if (nextNode.Count > 1)
        {
            StartCoroutine(ArrivePlayer(() =>ActiveArrow(true)));
            return false;
        }
        else
            node = nextNode[0];

        return true;
    }

    protected IEnumerator ArrivePlayer(Action action)
    {
        Transform p = MapControl.Instance.Curplayer.transform;

        while(true)
        {
            if (transform.position.Equals(p.position))
                break;

            yield return null;
        }

        action?.Invoke();
    }

    private void ActiveArrow(bool active)
    {
        foreach(var g in arrows)
            g.SetActive(active);
    }

    private void CreateArrow()
    {
        arrows = new();

        for (int i = 0; i < nextNode.Count; i++)
        {
            Vector3 pos = (nextNode[i].transform.position - transform.position) / 2;
            float angle = Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg;
            float dX = arrowPrefab.transform.localEulerAngles.x;

            BoardArrow a = Instantiate(arrowPrefab, transform.position + pos, Quaternion.Euler(dX, -angle + dX, 0));
            arrows.Add(a.gameObject);
            a.SetNode(nextNode[i]);

            a.OnEvent += () => ActiveArrow(false);
        }
    }
}
