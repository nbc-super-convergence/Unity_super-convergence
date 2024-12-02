using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMinigameResult : UIBase
{
    [SerializeField] private GameObject[] RankPanels;
    [SerializeField] private TextMeshProUGUI[] RankTxts;
    [SerializeField] private TextMeshProUGUI returnTxt;
    private Dictionary<int, int> coinDics = new()
    { { 1, 20 }, { 2, 10 }, { 3, 5 }, { 4, 1 } };

    public override void Opened(object[] param)
    {
        foreach (GameObject panel in RankPanels)
        {
            panel.SetActive(false);
        }

        //ranks의 string : sessionId, int : 등수
        if (param.Length > 0 && param[0] is Dictionary<string, int> ranks)
        {
            foreach (var rank in ranks)
            {
                string sessionid = rank.Key;
                int rankNum = rank.Value;
                int idx = GameManager.Instance.SessionDic[sessionid].Color;

                if (idx != -1)
                {    
                    RankPanels[idx].transform.SetSiblingIndex(rankNum - 1);

                    RankTxts[idx].text = $"{rankNum}등\n{GameManager.Instance.SessionDic[sessionid].Nickname}";
                    
                    //TODO : 코인 획득 UI
                    BoardManager.Instance.playerTokenHandlers[idx].data.keyAmount += coinDics[rankNum];

                    //미니게임 순서 재정의
                    GameManager.Instance.SessionDic[sessionid].SetOrder(rankNum - 1);
                }
                else
                {
                    RankPanels[idx].SetActive(false);
                    RankTxts[idx].text = "";
                }
            }
        }

        StartCoroutine(ReturnBoard());
    }

    private IEnumerator ReturnBoard()
    {
        int leftSeconds = 5;
        
        while (leftSeconds > 0)
        {
            returnTxt.text = $"{leftSeconds}초 후 보드로 돌아갑니다...";
            leftSeconds--;
            yield return new WaitForSeconds(1);
        }

        UIManager.Hide<UIMinigameResult>();
    }
}