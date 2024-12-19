using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameDart : UIBase
{
    #region UI 속성
    [Header("Force Power")]
    [SerializeField] private Slider forcePower;

    [Header("Round")]
    [SerializeField] private TextMeshProUGUI roundTxt;

    [Header("Result")]
    [SerializeField] private Image[] resultImage;
    [SerializeField] private TextMeshProUGUI[] stateTexts;
    [SerializeField] private GameObject[] scoreFields;
    private TextMeshProUGUI[,] scoreTexts;  //플레이어, 점수 2차원 배열
    #endregion

    private Color[] playerColor = {Color.red, Color.yellow, Color.green, Color.blue};

    private string nickname;

    private void Start()
    {
        //forcePower 초기
        SetForceLimit(1.5f, 3f);

        for (int i = 0; i < stateTexts.Length; i++)
        {
            stateTexts[i].color = playerColor[i];
            resultImage[i].color = Color.gray;
            SetReady(i);
        }

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            Transform List = scoreFields[i].transform;
            for (int j = 1; j <= 4; j++)
            {
                scoreTexts[i, j] = List.GetChild(j).GetComponentInChildren<TextMeshProUGUI>(); 
            }
        }
    }

    /// <summary>
    /// 현재 인원까지 보여주기
    /// </summary>
    /// <param name="idx"></param>
    public void ShowScoreField(int idx)
    {
        for (int i = 0; i < scoreFields.Length; i++)
        {
            if(i > idx) //현재 인원이 넘어가면
                scoreFields[i].gameObject.SetActive(false);
        }
    }

    public void AddScore(int color, int index, float distance)
    {
        scoreTexts[color, index].text = distance.ToString("N4");
    }

    #region Force 메서드
    public void SetForceLimit(float min, float max)
    {
        forcePower.minValue = min;
        forcePower.maxValue = max;
    }
    public void ChangeForcePower(float f)
    {
        forcePower.value = f;
    }
    public void ShowForcePower()
    {
        forcePower.gameObject.SetActive(true);
    }
    public void HideForcePower()
    {
        forcePower.gameObject.SetActive(false);
    }
    #endregion

    #region Result 메서드
    public void SetReady(int idx)
    {
        resultImage[idx].color = Color.gray;
        stateTexts[idx].color = playerColor[idx];
        ApplyText(idx, "준비");
    }
    public void SetMyTurn(int idx)
    {
        stateTexts[idx].color = Color.white;
        resultImage[idx].color = playerColor[idx];
        ApplyText(idx, "내 차례");
    }
    public void SetFinish(int idx)
    {
        ApplyText(idx, "OK!");
    }

    private void ApplyText(int idx, string txt)
    {
        stateTexts[idx].text = $"{idx+1}P : {txt}";
        //Debug.Log(stateTexts[idx].text);
    }
    #endregion

    public void SetRound(int round)
    {
        roundTxt.text = $"Round : {round}"; 
    }

    private void PlyaerLeftEvent(int color)
    {
        //이걸 어떤 용도로 구현?
    }

    //Todo : 내 차례가 되면 힘조절 UI 활성

    public override void Opened(object[] param)
    {
        GameManager.OnPlayerLeft += PlyaerLeftEvent;

        HashSet<int> usedColors = new HashSet<int>();
        foreach(var dic in GameManager.Instance.SessionDic)
        {
            int color = dic.Value.Color;
            usedColors.Add(color);
        }
        for (int i = 0; i < resultImage.Length; i++)
        {
            if(!usedColors.Contains(i))
            {
                resultImage[i].gameObject.SetActive(false);
            }
        }
    }
    public override void Closed(object[] param)
    {
        GameManager.OnPlayerLeft -= PlyaerLeftEvent;
    }
}
