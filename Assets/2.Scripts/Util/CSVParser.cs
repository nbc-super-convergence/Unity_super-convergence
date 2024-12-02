using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVParser : Singleton<CSVParser>
{
    public TextAsset csv;

    public Dictionary<int, string> failCodeDic = new Dictionary<int, string>();

    //private string[] m_Headers = new string[] { "Id", "Name", "IsBool", "Float", "CODE", "INFO", "KOR" };
    public async void Init()
    {
        //var result = await ResourceManager.Instance.LoadAsset<TextAsset>("HolyMolyPolyFailCode", eAddressableType.Data);
        //csv = result;
        ParseFailCode(csv);
        GameManager.Instance.failCodeDic = new(failCodeDic);
    }

    private string[] csvLine;
    private string[] splitData;
    private void ParseFailCode(TextAsset csv)
    {
        bool isFirstLine = true;
        csvLine = csv.text.Split('\n');

        foreach (string line in csvLine)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if(isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            splitData = line.Split(',');
            failCodeDic.Add(int.Parse(splitData[0]), splitData[2]);
        }
    }
}
