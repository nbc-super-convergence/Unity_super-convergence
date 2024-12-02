using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        foreach (var panel in RankPanels)
        {
            panel.gameObject.SetActive(false);
        }

        //ranks�� string : sessionId, int : ���
        if (param.Length > 0 && param[0] is Dictionary<string, int> ranks)
        {
            int i = 0;
            foreach (var rank in ranks)
            {
                string sessionid = rank.Key; //id
                int rankNum = rank.Value; //���
                int idx = GameManager.Instance.SessionDic[sessionid].Color; //����

                if (idx != -1)
                {    
                    //����� �´� ��ġ�� ���� ����
                    RankPanels[rankNum - 1].sprite = RankPanelsSprites[idx];

                    //��� + �г��� ����
                    RankTxts[rankNum - 1].text = $"{rankNum}��\n{GameManager.Instance.SessionDic[sessionid].Nickname}";
                    
                    //���� ����
                    BoardManager.Instance.playerTokenHandlers[idx].data.keyAmount += coinDics[rankNum];

                    //�̴ϰ��� ���� ������
                    GameManager.Instance.SessionDic[sessionid].SetOrder(rankNum - 1);
                }

                i++;
            }

            for (; i < 4; i++)
            {
                RankPanels[i].gameObject.SetActive(false);
                RankTxts[i].text = "";
            }
        }

        StartCoroutine(ReturnBoard());
    }

    private IEnumerator ReturnBoard()
    {
        int leftSeconds = 5;
        
        while (leftSeconds > 0)
        {
            returnTxt.text = $"{leftSeconds}�� �� ����� ���ư��ϴ�...";
            leftSeconds--;
            yield return new WaitForSeconds(1);
        }

        UIManager.Hide<UIMinigameResult>();
    }
}