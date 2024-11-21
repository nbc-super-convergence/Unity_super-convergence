using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : UIBase
{
    [SerializeField] private TMP_InputField inputFieldID;
    [SerializeField] private TMP_InputField inputFieldPassword;
    [SerializeField] private TMP_InputField inputFieldPasswordConfirm;
    [SerializeField] private TMP_InputField inputFieldNickname;

    [SerializeField] private Button buttonRegister;
    [SerializeField] private Button buttonBack;

    [SerializeField] private TMP_Text errorMessage;
    private StringBuilder sbError = new();

    public override void Opened(object[] param)
    {
        base.Opened(param);
        errorMessage.text = "";
    }


    private void Register()
    {
        if (!IsValidation())
        {
            errorMessage.text = sbError.ToString();
            sbError.Clear();
            return;
        }
        else
        {
            errorMessage.text = sbError.ToString();

            string inputID = inputFieldID.text;
            string inputPassword = inputFieldPassword.text;
            string inputpasswordConfirm = inputFieldPasswordConfirm.text;
            string inputNickname = inputFieldNickname.text;

            //GamePacket packet = new();
            //packet.AAAA = new()
            //{

            //};
            //SocketManager.Instance.OnSend(packet);

            //TODO:: ������ �����ؼ� ���� id�� ���� ��� �������� �ް�, �����޼��� ����.
            //await ����ޱ���� ��ٸ��� 
            if(IsSameID())
            {
                sbError.AppendLine($"���� ���̵� �����մϴ�.");
                errorMessage.text = sbError.ToString();
                sbError.Clear();
                return;
            }
            else
            {
                Debug.Log($"���Լ��� ID: {inputID}, Password: {inputPassword}");
                ButtonBack();
            }
        }
    }

    private bool IsValidation()
    {
        // 4���� �̻��� id, pw
        if (inputFieldID.text.Length < 4)
        {
            sbError.AppendLine($"���̵�� �ּ� 4�� �̻� �Է��ϼ���.");
            return false;
        }
        else if (inputFieldID.text.Length > 15)
        {
            sbError.AppendLine($"���̵�� �ִ� 15�� ���Ϸ� �Է��ϼ���.");
            return false;
        }
        if (inputFieldPassword.text.Length < 4)
        {
            sbError.Append($"��й�ȣ�� �ּ� 4�� �̻� �Է��ϼ���.");
            return false;
        }

        return true;
    }

    //TODO:: ������ �����ؼ� ���� id�� ���� ��� �������� �ް�, �����޼��� ����.
    private bool IsSameID()
    {
        return false;
    }


    #region Button
    public void ButtonBack()
    {
        UIManager.Hide<UIRegister>();
    }

    public void ButtonRegister()
    {
        Register();
    }
    #endregion
}
