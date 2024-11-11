using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    [Tooltip("¾À ÀÌµ¿ ½Ã : true ºñÆÄ±«/ false ÆÄ±«")]
    [SerializeField] protected bool isDontDestroyOnLoad = true;
    [NonSerialized] public bool isInitialized = false;
    protected static bool isUILoading = false;

    public static T Instance
    {
        get
        {
            if (!isUILoading)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance != null)
            Destroy(gameObject);
    }
}
