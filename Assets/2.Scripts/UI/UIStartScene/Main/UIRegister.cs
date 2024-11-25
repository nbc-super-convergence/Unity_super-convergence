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

            //TODO:: 서버에 전송해서 같은 id가 있을 경우 리스폰스 받고, 에러메세지 띄우기.
            //await 응답받기까지 기다리기 
            if(IsSameID())
            {
                sbError.AppendLine($"같은 아이디가 존재합니다.");
                errorMessage.text = sbError.ToString();
                sbError.Clear();
                return;
            }
            else
            {
                Debug.Log($"가입성공 ID: {inputID}, Password: {inputPassword}");
                ButtonBack();
            }
        }
    }

    private bool IsValidation()
    {
        // 4글자 이상의 id, pw
        if (inputFieldID.text.Length < 4)
        {
            sbError.AppendLine($"아이디는 최소 4자 이상 입력하세요.");
            return false;
        }
        else if (inputFieldID.text.Length > 15)
        {
            sbError.AppendLine($"아이디는 최대 15자 이하로 입력하세요.");
            return false;
        }
        if (inputFieldPassword.text.Length < 4)
        {
            sbError.Append($"비밀번호는 최소 4자 이상 입력하세요.");
            return false;
        }

        return true;
    }

    //TODO:: 서버에 전송해서 같은 id가 있을 경우 리스폰스 받고, 에러메세지 띄우기.
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
