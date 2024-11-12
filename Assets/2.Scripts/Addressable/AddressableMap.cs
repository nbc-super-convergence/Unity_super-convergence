using System.Collections.Generic;
using System;

[Serializable]
public class AddressableMapList
{// 엔트리의 에셋 주소들
    public List<AddressableMap> list = new List<AddressableMap>();

    public void AddRange(List<AddressableMap> list)
    {
        this.list.AddRange(list);
    }

    public void Add(AddressableMap data)
    {
        list.Add(data);
    }
}

[Serializable]
public class AddressableMap 
{// 단일 에셋 주소
    public eAddressableType addressableType;
    public eAssetType assetType;
    public string key;
    public string path;
}