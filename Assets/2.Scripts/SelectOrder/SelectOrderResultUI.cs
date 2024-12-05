using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectOrderResultUI : MonoBehaviour
{
    public List<TextMeshProUGUI> resultTexts;

    public void SetDistanceUI(int idx, float dist)
    {
        resultTexts[idx].text = $"{idx + 1}P : {dist.ToString("N4")}";
        switch(idx)
        {
            case 0:
                resultTexts[idx].color = Color.red; break;
            case 1:
                resultTexts[idx].color = Color.yellow; break;
            case 2:
                resultTexts[idx].color = Color.green; break;
            case 3:
                resultTexts[idx].color = Color.blue; break;
        }
    }
}