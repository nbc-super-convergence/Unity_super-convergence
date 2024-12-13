using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIMinigameResult : UIBase
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image[] RankPanels;
    [SerializeField] private Sprite[] RankPanelsSprites;
    [SerializeField] private TextMeshProUGUI[] RankTxts;
    [SerializeField] private TextMeshProUGUI[] CoinTxts;
    [SerializeField] private TextMeshProUGUI returnTxt;

    private Dictionary<int, int> colorIdxs = new();
    private Dictionary<int, int> coinDics = new()
    { { 1, 20 }, { 2, 10 }, { 3, 5 }, { 4, 1 } };

    private Sequence titleSequence;

    public override void Opened(object[] param)
    {
        GameManager.OnPlayerLeft += PlayerLeftEvent;
        MinigameManager.Instance.curMiniGame.DisableUI();

        foreach (var panel in RankPanels)
        {
            panel.gameObject.SetActive(false);
        }

        //ranks의 string : sessionId, int : 등수
        if (param.Length == 2)
        {
            if (param[0] is Dictionary<string, int> ranks)
            {
                int i = 0;
                foreach (var rank in ranks)
                {
                    string sessionid = rank.Key; //id
                    int rankNum = rank.Value; //등수
                    int color = GameManager.Instance.SessionDic[sessionid].Color; //색깔
                    colorIdxs.Add(color, rankNum); 

                    RankPanels[i].gameObject.SetActive(true);

                    //등수에 맞는 위치에 색깔 지정
                    RankPanels[i].sprite = RankPanelsSprites[color];

                    //등수 + 닉네임 설정
                    RankTxts[rankNum - 1].text = $"{rankNum}등\n{GameManager.Instance.SessionDic[sessionid].Nickname}";

                    //받을 금액 설정
                    string coin = rankNum switch
                    {
                        1 => "+20",
                        2 => "+10",
                        3 => "+5",
                        4 => "+1",
                        _ => "0"
                    };

                    i++;
                }
            }
            else
            {
                Debug.LogError("param parsing error : ranks");
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

        titleSequence = DOTween.Sequence();
        titleSequence.Append(titleText.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack));
        titleSequence.Append(titleText.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBounce));
        titleSequence.Join(titleText.transform.DOShakePosition(0.3f, new Vector3(10f, 0, 0), 20, 90, false, true));
    }

    public override async void Closed(object[] param)
    {
        titleSequence.Kill();

        colorIdxs.Clear();
        GameManager.OnPlayerLeft -= PlayerLeftEvent;

        await UIManager.Show<BoardUI>();
        BoardManager.Instance.NextTurn();
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
        yield return new WaitUntil(() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= returnTime);
        UIManager.Hide<UIMinigameResult>();
    }

    private void PlayerLeftEvent(int color)
    {
        RankPanels[colorIdxs[color]].color = new Color(145 / 255f, 145 / 255f, 145 / 255f, 220 / 255f);
        RankTxts[colorIdxs[color]].text = "오프라인";
        RankTxts[colorIdxs[color]].color = new Color(150 / 255f, 150 / 255f, 150 / 255f);
    }
}