using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public static bool IsSuccessLogin;

    [SerializeField] private TMP_InputField inputFieldID;
    [SerializeField] private TMP_InputField inputFieldPassward;

    [SerializeField] private Button buttonLogin;
    [SerializeField] private Button buttonRegister;

    [SerializeField] private TMP_Text errorMessage;
    private StringBuilder sbError = new();
    private WaitForSeconds errorSeconds = new WaitForSeconds(2f);


    public UIStart uiStart;

    #region Button
    // TODO:: ������ �α�����Ŷ ������ ���� �������� �ޱ�.
    public void ButtonLogin()
    {
        StartCoroutine(Error());
    }

    public async void ButtonRegister()
    {
        // TODO:: ���� �������� �ʰ� UIManager�� ���ϵ��� �ٲٱ�.
        if(uiStart.isFirst)
        {
            uiStart.DestroyUIStart();
        }
        else
        {
            UIManager.Hide<UIStart>();
        }
        await UIManager.Show<UIRegister>();
    }
    #endregion

    private IEnumerator Error()
    {
        sbError.AppendLine($"���� �̱���");
        errorMessage.text = sbError.ToString();
        sbError.Clear();
        buttonLogin.interactable = false;
        yield return errorSeconds;
        errorMessage.text = sbError.ToString();
        buttonLogin.interactable = true;
    }
}
