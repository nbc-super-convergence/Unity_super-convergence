using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : UIBase
{
    [SerializeField] private TMP_InputField inputFieldID;
    [SerializeField] private TMP_InputField inputFieldPassword;
    [SerializeField] private TMP_InputField inputFieldPasswordConfirm;
    [SerializeField] private TMP_InputField inputFieldNickname;
    [SerializeField] private Toggle passwordToggle;

    [SerializeField] private Button buttonRegister;
    [SerializeField] private Button buttonBack;

    [SerializeField] private TMP_Text errorMessage;
    private StringBuilder sbError = new();

    private TaskCompletionSource<bool> registerTcs;

    public override void Opened(object[] param)
    {
        errorMessage.text = "";

        passwordToggle.isOn = false;
        passwordToggle.onValueChanged.AddListener(ToggleInputFieldPassword);
    }


    private async void Register()
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
            string inputPasswordConfirm = inputFieldPasswordConfirm.text;
            string inputNickname = inputFieldNickname.text;

            GamePacket packet = new();
            packet.RegisterRequest = new()
            {
                LoginId = inputID,
                Password = inputPassword,
                PasswordConfirm = inputPasswordConfirm,
                Nickname = inputNickname
            };
            registerTcs = new();
            SocketManager.Instance.OnSend(packet);

            bool isSuccess = await registerTcs.Task;
            if (isSuccess)
            {
                Debug.Log($"가입성공 ID: {inputID}, Password: {inputPassword}");
                ButtonBack();                
            }
            else
            {
                //sbError.AppendLine($"같은 아이디가 존재합니다.");
                //errorMessage.text = sbError.ToString();
                //sbError.Clear();
                //return;
            }
        }
    }

    private bool IsValidation()
    {
        // 4글자 이상의 id, pw
        if (inputFieldID.text.Length < 4 || inputFieldID.text.Length > 12)
        {
            sbError.AppendLine($"아이디는 4자 이상, 12자 이하로 입력하세요.");
            return false;
        }        
        if (inputFieldPassword.text.Length < 4 || inputFieldPassword.text.Length > 16)
        {
            sbError.Append($"비밀번호는 4자 이상, 16자 이하로 입력하세요.");
            return false;
        }        
        if(inputFieldPassword.text != inputFieldPasswordConfirm.text)
        {
            sbError.Append($"입력하신 비밀번호와 확인이 일치하지 않습니다.");
            return false;
        }
        if(inputFieldNickname.text.Length < 4 || inputFieldNickname.text.Length > 12) 
        {
            sbError.Append($"닉네임은 4자 이상, 12자 이하로 입력하세요.");
            return false;
        }
        return true;
    }        

    public void TrySetTask(bool isSuccess)
    {
        bool boolll = registerTcs.TrySetResult(isSuccess);
        //Debug.Log(boolll ? "회원가입 성공" : "회원가입 실패");        
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


    private void ToggleInputFieldPassword(bool isOn)
    {
        if(isOn)
        {
            inputFieldPassword.contentType = TMP_InputField.ContentType.Standard;
            inputFieldPasswordConfirm.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            inputFieldPassword.contentType = TMP_InputField.ContentType.Password;
            inputFieldPasswordConfirm.contentType = TMP_InputField.ContentType.Password;
        }
        inputFieldPassword.ForceLabelUpdate();
        inputFieldPasswordConfirm.ForceLabelUpdate();
    }
}
