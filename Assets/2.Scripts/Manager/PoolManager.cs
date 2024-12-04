using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    Dictionary<string, Queue<ObjectPoolBase>> pools = new Dictionary<string, Queue<ObjectPoolBase>>();
    public List<ObjectPoolBase> prefabList = new();
    public bool isInit = false;

    public async void Init()
    {
        foreach (var prefab in prefabList)
        {
            prefab.data.prefab = await ResourceManager.Instance.LoadAsset<ObjectPoolBase>(prefab.data.rCode, eAddressableType.Prefab);
            prefab.data.parent = new GameObject(prefab.data.rCode + "parent").transform;
            prefab.data.parent.parent = transform;
            Queue<ObjectPoolBase> queue = new Queue<ObjectPoolBase>();
            pools.Add(prefab.data.rCode, queue);
            for (int i = 0; i < prefab.data.count; i++)
            {
                var obj = Instantiate(prefab.data.prefab, prefab.data.parent);
                obj.name = obj.name.Replace("(Clone)", "");
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
        }
        isInit = true;
        isInitialized = true;
    }

    public T Spawn<T>(string rcode, params object[] param) where T : ObjectPoolBase
    {
        if (pools[rcode].Count == 0)
        {
            var prefab = prefabList.Find(obj => obj.data.rCode == rcode);
            for (int i = 0; i < prefab.data.count; i++)
            {
                var obj = Instantiate(prefab.data.prefab, prefab.data.parent);
                obj.name.Replace("(Clone)", "");
                pools[rcode].Enqueue(obj);
            }
        }
        var retObj = (T)pools[rcode].Dequeue();
        retObj.SetActive(true);
        retObj.Init(param);
        return retObj;
    }

    public T Spawn<T>(string rcode, Transform parent, params object[] param) where T : ObjectPoolBase
    {
        var obj = Spawn<T>(rcode, param);
        obj.transform.parent = parent;
        return obj;
    }

    public T Spawn<T>(string rcode, Vector3 position, Transform parent, params object[] param) where T : ObjectPoolBase
    {
        var obj = Spawn<T>(rcode, parent, param);
        obj.transform.position = position;
        return obj;
    }

    public T Spawn<T>(string rcode, Vector3 position, Quaternion rotation, Transform parent, params object[] param) where T : ObjectPoolBase
    {
        var obj = Spawn<T>(rcode, position, parent, param);
        obj.transform.rotation = rotation;
        return obj;
    }

    public void Release(ObjectPoolBase item)
    {
        item.SetActive(false);
        var prefab = prefabList.Find(obj => obj.data.rCode == item.name);
        item.transform.parent = prefab.data.parent;
        pools[item.name].Enqueue(item);
    }

    public T SpawnFromPool<T>(string rcode) where T : ObjectPoolBase
    {
        if (!pools.ContainsKey(rcode)) return default;

        ObjectPoolBase obj = pools[rcode].Dequeue();
        pools[rcode].Enqueue(obj);
        obj.gameObject.SetActive(true);
        return (T)obj;
    }
}
