using System.Collections;
using TMPro;
using UnityEngine;

public class UIMinigameIce : UIBase
{
    [SerializeField] private TextMeshProUGUI timeTxt;
    //TODO : HP
    //[SerializeField] private 

    private GameIceSliderData gameData;

    public override void Opened(object[] param)
    {
        gameData = (GameIceSliderData)param[0];
        StartCoroutine(UIUtils.DecreaseTimeCoroutine(gameData.totalTime, timeTxt));
    }

    public void ChangeHPUI()
    {

    }

    public void CheckTime()
    {

    }

    //HP SET
}