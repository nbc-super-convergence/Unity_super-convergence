using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UIError : UIBase
{
    [SerializeField] private TMP_Text errorMessage;

    private StringBuilder sbError = new();

    //private bool isPopup = false;

    //public override void Opened(object[] param)
    //{
    //    sbError.Clear();
    //    sbError.Append(param[0].ToString());
    //    errorMessage.text = sbError.ToString();
    //    //isPopup = true;
    //    StartCoroutine(Close());
    //}

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

    public void SetErrorMessage(int errorNum)
    {
        sbError.Clear().AppendLine( ((ERROR_MESSAGE)errorNum).ToString() );
        errorMessage.text = sbError.ToString();
        SetActive(true);
    }

    /*
    0. 서버로부터 에러코드를 받는다.
    1. 에러코드를 띄울 곳에서 UIManager.Show<UIError>(); 실행.
    2. UIError가 isActiveInCreated = false로 로드된다.
    3. 에러코드를 띄울 곳에서 UIManager.Get<UIError>().[TMP_Text.text를 바꾸는 메서드](매개변수 서버에서 받은 정보); 실행
    4. UIManager.Get<UIError>().SetActive(true);
    */
}

public enum ERROR_MESSAGE
{
    WRONG_MESSAGE,
    IDONTKNOW
}
