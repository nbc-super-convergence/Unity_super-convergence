using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolBase : MonoBehaviour
{
    public ObjectPoolData data;
    public abstract void Init(params object[] param);

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Release()
    {
        PoolManager.Instance.Release(this);
    }
}

[System.Serializable]
public class ObjectPoolData
{
    public string rCode;
    public int count = 50;
    public int velocity = 25;
    [HideInInspector] public ObjectPoolBase prefab;
    [HideInInspector] public Transform parent;
}