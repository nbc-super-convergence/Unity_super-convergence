using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectOrderResultUI : MonoBehaviour
{
    public int player = 1;
    private Image bgImage;
    private TextMeshProUGUI stateText;
    public string Nickname { get; private set; }
    private string state;

    // 닉네임 : 상태 (준비중 / 내차례 / 거리)

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        stateText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        bgImage.color = Color.gray;
        
        switch(player)  //플레이어 색상
        {
            case 1: stateText.color = Color.red; break;
            case 2: stateText.color = Color.yellow; break;
            case 3: stateText.color = Color.green; break;
            case 4: stateText.color = Color.blue; break;
            default: stateText.color = Color.gray; break;
        }

        SetReady();
    }

    private void Update()
    {
        ApplyText();
    }

    private void ApplyText()
    {
        stateText.text = $"{Nickname} : {state}";
    }

    public void SetReady() => state = "준비";
    public void SetMyTurn() => state = "내 차례";
    public void SetFinish(float dist) => state = dist.ToString("N4");
}