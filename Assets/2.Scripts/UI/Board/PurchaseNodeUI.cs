using System;
using TMPro;

public class PurchaseNodeUI : UIBase
{
    public TextMeshProUGUI text;
    private int requireAmount = 0;
    private IPurchase action;
    private int index;

    public override void Opened(object[] param)
    {
        base.Opened(param);
        text.text = $"{requireAmount}�� ���踦 �����Ͽ� ���Ű� �����մϴ�.";

        action = (IPurchase)param[0];
        index = (int)param[1];
    }

    //���� ���
    public void Accept()
    {
        var p = MapControl.Instance.playerTokenHandlers[index];

        if(p.data.keyAmount > requireAmount)
        {
            p.data.keyAmount -= requireAmount;
            action.Purchase(index);
        }

        SetActive(false);
    }

}
