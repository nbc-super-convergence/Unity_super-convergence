using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMinigameResult : UIBase
{
    [SerializeField] private Image[] RankPanels;
    [SerializeField] private Sprite[] RankPanelsSprites;

    [SerializeField] private TextMeshProUGUI[] RankTxts;
    [SerializeField] private TextMeshProUGUI returnTxt;
    private Dictionary<int, int> coinDics = new()
    { { 1, 20 }, { 2, 10 }, { 3, 5 }, { 4, 1 } };

    public override void Opened(object[] param)
    {
        UIManager.Hide<UIMinigameIce>();

        foreach (var panel in RankPanels)
        {
            panel.gameObject.SetActive(false);
        }

        //ranks의 string : sessionId, int : 등수
        if (param.Length == 2)
        {
            if (param[0] is Dictionary<string, int> ranks)
            {
                foreach (var rank in ranks)
                {
                    string sessionid = rank.Key; //id
                    int rankNum = rank.Value; //등수
                    int color = GameManager.Instance.SessionDic[sessionid].Color; //색깔

                    RankPanels[color].gameObject.SetActive(true);

                    //등수에 맞는 위치에 색깔 지정
                    RankPanels[rankNum - 1].sprite = RankPanelsSprites[color];

                    //등수 + 닉네임 설정
                    RankTxts[rankNum - 1].text = $"{rankNum}등\n{GameManager.Instance.SessionDic[sessionid].Nickname}";

                    //보상 지급
                    BoardManager.Instance.playerTokenHandlers[color].data.keyAmount += coinDics[rankNum];

                    //미니게임 순서 재정의
                    GameManager.Instance.SessionDic[sessionid].SetOrder(rankNum - 1);
                }
            }
            else
            {
                Debug.LogError("param 오류 : idx0이 ranks가 아님");
            }

            if (param[1] is long returnTime)
            {
                StartCoroutine(ReturnTxt());
                StartCoroutine(ReturnBoard(returnTime));
            }
        }
        else
        {
            Debug.LogError("param 오류 : object[] length가 다름");
        }
    }

    private IEnumerator ReturnTxt()
    {
        int leftSeconds = 5;
        
        while (leftSeconds > 0)
        {
            returnTxt.text = $"{leftSeconds}초 후 보드로 돌아갑니다...";
            leftSeconds--;
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator ReturnBoard(long returnTime)
    {
        yield return new WaitUntil(() => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= returnTime);
        UIManager.Hide<UIMinigameResult>();
    }
}