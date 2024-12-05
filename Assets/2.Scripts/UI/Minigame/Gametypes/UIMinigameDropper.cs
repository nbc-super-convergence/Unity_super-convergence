using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameDropper : UIBase
{
    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private Image[] hpBars;

    private GameDropperData gameData;

    public override void Opened(object[] param)
    {
        gameData = (GameDropperData)param[0];
        //StartCoroutine(UIUtils.DecreaseTimeCoroutine(gameData.totalTime, timeTxt));
    }

    public void ChangeHPUI()
    {
        for (int i = 0; i < hpBars.Length; i++)
        {
            hpBars[i].fillAmount = gameData.playerHps[i] / gameData.maxHP;
        }
    }
}