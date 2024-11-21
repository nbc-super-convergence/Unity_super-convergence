using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : UIBase
{
    public static bool IsSuccessLogin;

    [SerializeField] private TMP_InputField inputFieldID;
    [SerializeField] private TMP_InputField inputFieldPassward;

    [SerializeField] private Button buttonLogin;
    [SerializeField] private Button buttonRegister;

    [SerializeField] private TMP_Text errorMessage;
    private StringBuilder sbError = new();
    private WaitForSeconds errorSeconds = new WaitForSeconds(2f);


    public override void Opened(object[] param)
    {
        errorMessage.text = "";
    }


    private async void Login()
    {
        // TODO:: 서버로 로그인패킷 보내고 성공/실패 리스폰스 받기.
        if(IsSuccessLogin) 
        {
            // 로그인 입력화면 비활성화
            UIManager.Hide<UILogin>();
            // 로그아웃버튼 활성화
            await UIManager.Show<UILogout>();
        }
        else
        {
            StartCoroutine(FailLogin());
            return;
        }
    }

    private IEnumerator FailLogin()
    {
        sbError.AppendLine($"아직 미구현");
        errorMessage.text = sbError.ToString();
        sbError.Clear();
        buttonLogin.interactable = false;
        yield return errorSeconds;
        errorMessage.text = sbError.ToString();
        buttonLogin.interactable = true;
    }


    #region Button
    public void ButtonLogin()
    {
        Login();
    }

    public async void ButtonRegister()
    {
        UIManager.Hide<UIStart>();
        await UIManager.Show<UIRegister>();
    }
    #endregion
    
}
