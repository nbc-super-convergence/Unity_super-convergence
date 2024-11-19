using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UIError : UIBase
{
    [SerializeField] private TMP_Text errorMessage;

    private StringBuilder sbError = new();

    //private bool isPopup = false;

    public override void Opened(object[] param)
    {
        sbError.Clear();
        sbError.Append(param[0].ToString());
        errorMessage.text = sbError.ToString();
        //isPopup = true;
        StartCoroutine(Close());
    }

    public override void Closed(object[] param)
    {
        StopCoroutine(Close());
    }


    public async void ShowError(string message) // 또는 에러메세지
    {
        await UIManager.Show<UIError>();
    }

    private IEnumerator Close()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Hide<UIError>();
    }
}
