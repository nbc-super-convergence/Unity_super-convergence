using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameDropper : UIBase
{
    private GameDropperData gameData;

    public override void Opened(object[] param)
    {
        if (param.Length == 2)
        {
            if (param[0] is GameDropperData data)
            {
                gameData = data;
            }
            else
            {
                Debug.LogError("param parsing error : GameDropperData");
                return;
            }

            if (param[1] is long startTime)
            {
                StartCoroutine(StartCountDown(startTime));
            }
            else
            {
                Debug.LogError("param parsing error : startTime");
                return;
            }
        }
        else
        {
            Debug.LogError("param length error");
            return;
        }
    }

    private IEnumerator StartCountDown(long startdelay)
    {
        //TODO : Ready Text 띄우기

        yield return new WaitUntil(() => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= startdelay); //시작시간 대기

        //TODO : Start 글씨 잠깐 띄우기 없애기
        //TODO : 최초의 구멍 setactive false
    }
}