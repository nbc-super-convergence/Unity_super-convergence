using System;
using TMPro;
using UnityEngine;

public class PurchaseNodeUI : UIBase
{
    [SerializeField] TextMeshProUGUI content;
    //[SerializeField] TextMeshProUGUI price;

    private IPurchase action;

    public override void Opened(object[] param)
    {
        base.Opened(param);
        action = (IPurchase)param[0];

        content.text = action.message;
        //price.text = action.price.ToString();
    }

    //���� ���
    public void Accept()
    {
        //�ӽ� �ּ�
        //if (p.data.keyAmount > requireAmount)
        //{
        //    p.data.keyAmount -= requireAmount;
            action.Purchase();
        //}
        //else Cancle();
        //Cancle();

        SetActive(false);
    }

    public void Cancle()
    {
        action.Cancle();
        SetActive(false);
    }
}
