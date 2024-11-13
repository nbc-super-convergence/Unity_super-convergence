using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ResourceManager : Singleton<ResourceManager>
{
    public Dictionary<eAddressableType, Dictionary<string, object>> assetPools = new();
    private readonly Dictionary<eAddressableType, Dictionary<string, AddressableMap>> addressableMaps = new();

    [NonSerialized] public bool isInit = false;

    //GameManager해서 호출함으로써 Manager간 초기화 서순 지키기.
    public IEnumerator Init()
    {
        //Initialize Addressable
        yield return Addressables.InitializeAsync();

        //yield return DownloadInitAssets(); 

        //Init Addressable Map
        InitAddressableMap();

        //ResourceManager Initialized.
        isInitialized = true;
    }

    #region Init Addressable Assets
    /// <summary> 
    /// 서버로부터 Addressable Asset 다운로드,
    /// 게임 첫 실행 시 한 번만 호출
    /// </summary>
    private IEnumerator DownloadInitAssets()
    {
        //Download Addressable from Server
        var handle = Addressables.DownloadDependenciesAsync("InitDownload");
        yield return SetProgress(handle);

        switch (handle.Status)
        {
            case AsyncOperationStatus.None:
                break;
            case AsyncOperationStatus.Succeeded:
                Debug.Log("다운로드 성공");
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError("다운로드 실패 :\n" + handle.OperationException.Message);
                Debug.LogError(handle.OperationException.ToString());
                break;
            default:
                break;
        }
        
        Addressables.Release(handle);
    }

    //추후 UI 로딩 슬라이드 바와 연동
    private IEnumerator SetProgress(AsyncOperationHandle handle)
    {
        while (!handle.IsDone)
        {
            //UILoading.instance.SetProgress(handle.GetDownloadStatus().Percent, "Resource Download...");
            yield return new WaitForEndOfFrame();
        }
        //UILoading.instance.SetProgress(1);

    }

    //어드레서블 맵 캐싱
    private async void InitAddressableMap()
    {
        await Addressables.LoadAssetsAsync<TextAsset>("AddressableMap", (text) =>
        {
            AddressableMapList mapList = JsonUtility.FromJson<AddressableMapList>(text.text);
            eAddressableType key = eAddressableType.Prefab;
            Dictionary<string, AddressableMap> mapDic = new Dictionary<string, AddressableMap>();
            foreach (AddressableMap data in mapList.list)
            {
                key = data.addressableType;
                if (!mapDic.ContainsKey(data.key))
                    mapDic.Add(data.key, data);
            }
            if (!addressableMaps.ContainsKey(key)) addressableMaps.Add(key, mapDic);

        }).Task;
        isInit = true;
    }
    #endregion

    #region Addressable Asset Loading
    private string GetAssetPath(string key, eAddressableType addressableType)
    {
        var map = addressableMaps[addressableType][key.ToLower()];
        return map.path;
    }

    private List<string> GetAssetPaths(eAddressableType group, eAssetType assetType)
    {
        var keys = new List<string>(addressableMaps[group].Keys);
        List<string> pathList = new List<string>();

        keys.ForEach(key =>
        {
            if (addressableMaps[group][key].assetType == assetType)
            {
                pathList.Add(addressableMaps[group][key].path);
            }
                
        });

        return pathList;
    }

    /// <summary>
    /// Load a single asset, cached in assetPool
    /// </summary>
    /// <typeparam name="T">class name</typeparam>
    /// <param name="key">item name</param>
    /// <param name="group">addressable group name</param>
    /// <returns>T object</returns>
    public async Task<T> LoadAsset<T>(string key, eAddressableType group) where T : UnityEngine.Object
    {
        //UI : UIList에서 관리. 나머지 : assetPool에서 캐싱.
        if (group != eAddressableType.UI && assetPools[group].ContainsKey(key))
            return (T)assetPools[group][key];

        var path = GetAssetPath(key, group);
        return await LoadAssetAsync<T>(path);
    }
    
    /// <summary>
    /// Load all assets by extension, not cached
    /// </summary>
    /// <typeparam name="T">class name</typeparam>
    /// <param name="key">item name</param>
    /// <param name="group">addressable group name</param>
    /// <param name="extension">file extension type</param>
    /// <returns>T object List</returns>
    public async Task<List<T>> LoadAssetList<T>(eAddressableType group, eAssetType extension) where T : UnityEngine.Object
    {
        List<T> objList = new List<T>();
        List<string> paths = GetAssetPaths(group, extension);

        foreach (var path in paths)
        {
            objList.Add(await LoadAssetAsync<T>(path));
        }

        return objList;
    }

    /// <summary>
    /// 반환 형식 커스텀해주는 함수
    /// </summary>
    private async Task<T> LoadAssetAsync<T>(string path)
    {
        if (path.Contains(".prefab") && typeof(T) != typeof(GameObject) || path.Contains("UI/"))
        {
            var obj = await Addressables.LoadAssetAsync<GameObject>(path).Task;
            return obj.GetComponent<T>();
        }
        else if (path.Contains(".json"))
        {
            var textAsset = await Addressables.LoadAssetAsync<TextAsset>(path).Task;
            return JsonUtility.FromJson<T>(textAsset.text);
        }
        else
        {
            return await Addressables.LoadAssetAsync<T>(path).Task;
        }
    }
    #endregion
}