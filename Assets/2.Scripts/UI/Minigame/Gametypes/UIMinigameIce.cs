using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameIce : UIBase
{

    [SerializeField] private TextMeshProUGUI timeTxt;

    [Header("HP Bar")]
    [SerializeField] private CanvasGroup[] hpParent;
    [SerializeField] private Image[] hpBars;
    [SerializeField] private GameObject[] offlineTxt;

    private GameIceSliderData gameData;

    public override void Opened(object[] param)
    {
        GameManager.OnPlayerLeft += PlayerLeftEvent;

        //플레이어 수 만큼 hp바 키기
        HashSet<int> usedColors = new HashSet<int>();
        foreach (var dic in GameManager.Instance.SessionDic)
        {
            int color = dic.Value.Color;
            usedColors.Add(color);

            hpParent[color].alpha = 1;
            hpBars[color].fillAmount = 1;
        }
        for (int i = 0; i < hpParent.Length; i++)
        {
            if (!usedColors.Contains(i))
            {
                hpParent[i].gameObject.SetActive(false);
            }
        }

        gameData = (GameIceSliderData)param[0];
        StartCoroutine(UIUtils.DecreaseTimeCoroutine(gameData.totalTime, timeTxt));
    }

    public override void Closed(object[] param)
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
    }

    public void ChangeHPUI()
    {
        for (int i = 0; i < hpBars.Length; i++)
        {
            hpBars[i].fillAmount = gameData.playerHps[i] / gameData.maxHP;
        }
    }

    private void PlayerLeftEvent(int color)
    {
        hpParent[color].alpha = 0.2f;
        hpBars[color].fillAmount = 0;
        offlineTxt[color].SetActive(true);
    }
}