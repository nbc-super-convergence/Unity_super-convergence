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

    [SerializeField] private Button registerBtn;
    [SerializeField] private Button backBtn;

    private TaskCompletionSource<bool> registerTcs;

    public override void Opened(object[] param)
    {
        passwordToggle.isOn = false;
        passwordToggle.onValueChanged.AddListener(ToggleInputFieldPassword);
        inputFieldPassword.onValueChanged.AddListener(OnPasswordInputChanged);
        inputFieldPasswordConfirm.onValueChanged.AddListener(OnPasswordConfirmChanged);

    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { OnBackBtn(); }
    }


    private async void Register()
    {
        if (!IsValidation())
        {            
            return;
        }
        else
        {
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
                OnBackBtn();                
            }
            else
            {
            }
        }
    }

    private bool IsValidation()
    {
#pragma warning disable CS4014
        if (inputFieldNickname.text.Length < 2 || inputFieldNickname.text.Length > 10)
        {
            UIManager.Show<UIError>("닉네임은 2자 이상, 10자 이하로 입력하세요.");
            return false;
        }
        if (inputFieldID.text.Length < 4 || inputFieldID.text.Length > 12)
        {
            UIManager.Show<UIError>("아이디는 4자 이상, 12자 이하로 입력하세요.");
            return false;
        }        
        if (inputFieldPassword.text.Length < 4 || inputFieldPassword.text.Length > 16)
        {
            UIManager.Show<UIError>("비밀번호는 4자 이상, 16자 이하로 입력하세요.");
            return false;
        }        
        if(inputFieldPassword.text != inputFieldPasswordConfirm.text)
        {
            UIManager.Show<UIError>("입력하신 비밀번호와 확인이 일치하지 않습니다.");
            return false;
        }        
        return true;
#pragma warning restore CS4014
    }

    public void TrySetTask(bool isSuccess)
    {
        bool b = registerTcs.TrySetResult(isSuccess);
    }

    #region Button
    public void OnBackBtn()
    {
        UIManager.Hide<UIRegister>();
    }

    public void OnRegisterBtn()
    {
        Register();
    }
    #endregion

    private void OnPasswordInputChanged(string value)
    {
        inputFieldPassword.text = Utils.FilterToASCII(value);
    }

    private void OnPasswordConfirmChanged(string value)
    {
        inputFieldPasswordConfirm.text = Utils.FilterToASCII(value);
    }

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
