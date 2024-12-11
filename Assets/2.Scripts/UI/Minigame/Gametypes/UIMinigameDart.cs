using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameDart : UIBase
{
    [Header("Backgrounds")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image howTo;

    [Header("Force Power")]
    [SerializeField] private Slider forcePower;

    [Header("Result")]
    [SerializeField] private Image[] resultImage;
    [SerializeField] private TextMeshProUGUI[] stateTexts;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;

    private Color[] playerColor = {Color.red, Color.yellow, Color.green, Color.blue};

    private void Start()
    {
        //forcePower 초기
        forcePower.minValue = 1.5f;
        forcePower.maxValue = 3f;

        for (int i = 0; i < stateTexts.Length; i++)
        {
            stateTexts[i].color = playerColor[i];
            resultImage[i].color = Color.gray;
            SetReady(i);
        }
    }

    public void SetForceLimit(float min, float max)
    {
        forcePower.minValue = min;
        forcePower.maxValue = max;
    }

    private void SetReady(int idx)
    {
        ApplyText(idx, "준비");
    }
    private void SetMyTurn(int idx)
    {
        stateTexts[idx].color = Color.white;
        resultImage[idx].color = playerColor[idx];
        ApplyText(idx, "내 차례");
    }
    private void SetFinish(int idx)
    {
        ApplyText(idx, "OK!");
    }
    private void SetScore(int idx, float score)
    {
        if (score >= 10f)
            ApplyText(idx, "Miss");
        else
            ApplyText(idx, score.ToString("N4"));
    }

    private void ApplyText(int idx, string txt)
    {
        //GameManager.Instance.myInfo.Nickname;
        string nickname = "user"; //사용자 이름 (임시로)
        stateTexts[idx].text = $"{nickname} : {txt}";
    }
}
