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
        // TODO:: ������ �α�����Ŷ ������ ����/���� �������� �ޱ�.
        if(IsSuccessLogin) 
        {
            // �α��� �Է�ȭ�� ��Ȱ��ȭ
            UIManager.Hide<UILogin>();
            // �α׾ƿ���ư Ȱ��ȭ
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
        sbError.AppendLine($"���� �̱���");
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
