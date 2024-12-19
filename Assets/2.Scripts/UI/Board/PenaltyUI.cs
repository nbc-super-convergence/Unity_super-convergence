using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PenaltyUI : UIBase
{
    private int penalty,tax;
    public TMP_Text message;
    private List<StringBuilder> messages = new();
    private IPurchase purchase;
    private BoardTokenData data;

    protected override void Awake()
    {
        base.Awake();

        for(int i = 0; i < 2; i++)
            messages.Add(new StringBuilder(""));
    }

    public override void Opened(object[] param)
    {
        base.Opened(param);
        penalty = (int)param[0];
        purchase = (IPurchase)param[1];
        data = (BoardTokenData)param[2];

        gameObject.SetActive(false);
    }

    public override void Closed(object[] param)
    {
        base.Closed(param);
    }

    public void SetTax(int prev,int t)
    {
        tax = t;
        MessageUpdate();

        SetActive(true);
        StartCoroutine(PenaltyEvent(prev < (penalty >> 1)));
    }

    private IEnumerator PenaltyEvent(bool isGreater)
    {
        int count = isGreater ? 2 : 1;

        for (int i = 0; i < count; i++)
        {
            message.text = messages[i].ToString();
            yield return new WaitForSeconds(2.0f);
        }

        UIManager.Hide<PenaltyUI>();

        if (data.coin >= (penalty * 1.5f)) Purchase();
        else BoardManager.Instance.TurnEnd();
    }

    private async void Purchase()
    {
        await UIManager.Show<PurchaseNodeUI>(purchase);
    }

    private void MessageUpdate()
    {
        for (int i = 0; i < messages.Count; i++)
            messages[i].Clear();

        messages[0].Append($"어서오세요, 통행료는 {penalty >> 1}입니다.");
        messages[1].Append($"돈이 부족하시군요? {tax}를 지불하고 가세요 에휴");
    }
}
