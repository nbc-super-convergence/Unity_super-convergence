using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseNodeUI : UIBase
{
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] Button[] buttons = new Button[2];
    //[SerializeField] TextMeshProUGUI price;

    private IPurchase action;

    public override void Opened(object[] param)
    {
        base.Opened(param);

        Active(true);
        action = (IPurchase)param[0];

        content.text = action.message;
        //price.text = action.price.ToString();
    }

    //구매 기능
    public void Accept()
    {
        //임시 주석
        //if (p.data.keyAmount > requireAmount)
        //{
        //    p.data.keyAmount -= requireAmount;
            action?.Purchase();
        //}
        //else Cancle();
        //Cancle();
        Active(false);
        UIManager.Hide<PurchaseNodeUI>();
    }

    public void Cancle()
    {
        Active(false);
        action.Cancle();
        UIManager.Hide<PurchaseNodeUI>();
    }

    private void Active(bool isActive)
    { 
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = isActive;
        }
    }
}
