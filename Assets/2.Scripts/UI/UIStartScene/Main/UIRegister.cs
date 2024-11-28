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

    [SerializeField] private Button buttonRegister;
    [SerializeField] private Button buttonBack;

    [SerializeField] private TMP_Text errorMessage;
    private StringBuilder sbError = new();

    private TaskCompletionSource<bool> registerTcs;

    public override void Opened(object[] param)
    {
        base.Opened(param);
        errorMessage.text = "";
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
                Debug.Log($"���Լ��� ID: {inputID}, Password: {inputPassword}");
                ButtonBack();                
            }
            else
            {
                //sbError.AppendLine($"���� ���̵� �����մϴ�.");
                //errorMessage.text = sbError.ToString();
                //sbError.Clear();
                //return;
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

    public void TrySetTask(bool isSuccess)
    {
        bool boolll = registerTcs.TrySetResult(isSuccess);
        Debug.Log(boolll ? "ȸ������ ����" : "ȸ������ ����");        
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
