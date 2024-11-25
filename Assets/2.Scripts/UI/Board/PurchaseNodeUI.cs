using System;
using TMPro;
using UnityEngine;

public class PurchaseNodeUI : UIBase
{
    [SerializeField] TextMeshProUGUI content;
    //[SerializeField] TextMeshProUGUI price;

    private IPurchase action;
    private int index;

    public override void Opened(object[] param)
    {
        base.Opened(param);
        action = (IPurchase)param[0];
        index = (int)param[1];

        content.text = action.message;
        //price.text = action.price.ToString();
    }

    //���� ���
    public void Accept()
    {
        var p = BoardManager.Instance.playerTokenHandlers[index];

        //�ӽ� �ּ�
        //if(p.data.keyAmount > requireAmount)
        //{
        //    p.data.keyAmount -= requireAmount;
            action.Purchase(index);
        //}

        SetActive(false);
    }

    public void Cancle()
    {
        action.Cancle();
        SetActive(false);
    }
}
