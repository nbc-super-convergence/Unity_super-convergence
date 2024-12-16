using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILogin : UIBase
{
    public static bool isSuccessLogin = false;

    [SerializeField] private TMP_InputField IDInput;
    [SerializeField] private TMP_InputField PasswardInput;

    [SerializeField] private TextMeshProUGUI versionTxt;
    private StringBuilder sbError = new();

    private TaskCompletionSource<bool> sourceTcs;

    public override void Opened(object[] param)
    {
        SoundManager.Instance.PlayBGM(BGMType.Login);
        versionTxt.text = "current version : " + Application.version;
    }


    #region Button
    //Inspector: 회원가입 판넬 키기
    public async void OnRegisterBtn()
    {
        await UIManager.Show<UIRegister>();
    }

    //Inspector: 로그인 후 로비로 들어가기
    public async void OnLoginBtn()
    {
        if (!IsValidation())
        {
            await UIManager.Show<UIError>(sbError.ToString());
            sbError.Clear();
            return;
        }

        ////Send: 서버로 ID PW.
        string id = IDInput.text;
        string passward = PasswardInput.text;

        GamePacket packet = new();
        packet.LoginRequest = new()
        {
            LoginId = id,
            Password = passward
        };
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);

        ////Receive: 서버로부터 로그인 유효성 검사.
        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {
            IDInput.text = "";
            PasswardInput.text = "";
            IDInput.ForceLabelUpdate();
            PasswardInput.ForceLabelUpdate();

            await UIManager.Show<UILobby>();
            UIManager.Hide<UILogin>();
        }
    }

    public void TrySetTask(bool isSuccess)
    {
        if (sourceTcs.TrySetResult(isSuccess))
        {            
        }
    }    

    //Inspector: 게임종료 판넬 키기
    public async void OnQuitBtn()
    {
        await UIManager.Show<UIQuit>();
    }
    #endregion
    private bool IsValidation()
    {
        if (IDInput.text.Length < 4 || IDInput.text.Length > 12)
        {
            sbError.AppendLine($"아이디는 4자 이상, 12자 이하로 입력하세요.");
            return false;
        }
        if (PasswardInput.text.Length < 4 || PasswardInput.text.Length > 16)
        {
            sbError.Append($"비밀번호는 4자 이상, 16자 이하로 입력하세요.");
            return false;
        }

        return true;
    }

    #region Test Zone
    private bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(path);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        Debug.Assert(false, $"Scene {sceneName} does not exist in the [Scenes In Build]. Pleas Check [Build Setting...]");
        return false;
    }

    private void SendPacketIceJoinRequest()
    {
        GamePacket packet = new();
        //packet.IceJoinRequest = new();
        SocketManager.Instance.OnSend(packet);
    }
    #endregion
}
