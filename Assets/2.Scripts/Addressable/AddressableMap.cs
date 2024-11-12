using System.Collections.Generic;
using System;

[Serializable]
public class AddressableMapList
{// ��Ʈ���� ���� �ּҵ�
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
{// ���� ���� �ּ�
    public eAddressableType addressableType;
    public eAssetType assetType;
    public string key;
    public string path;
}