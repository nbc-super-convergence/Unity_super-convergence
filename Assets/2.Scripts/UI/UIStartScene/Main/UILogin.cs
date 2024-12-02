using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogin : UIBase
{
    public static bool isSuccessLogin = false;

    [SerializeField] private TMP_InputField IDInput;
    [SerializeField] private TMP_InputField PasswardInput;

    [SerializeField] private TextMeshProUGUI versionTxt;

    private TaskCompletionSource<bool> sourceTcs;

    private void Start()
    {
        versionTxt.text = "current version : " + Application.version;
    }

    #region Button
    //Inspector: ȸ������ �ǳ� Ű��
    public async void OnRegisterBtn()
    {
        await UIManager.Show<UIRegister>();
    }

    //Inspector: �α��� �� �κ�� ����
    public async void OnLoginBtn()
    {
        //�׽�Ʈ �ڵ�
        //if (IsSceneInBuild(targetScene))
        //{
        //    SocketManager.Instance.Init();
        //    SendPacketIceJoinRequest();
        //    SceneManager.LoadScene(targetScene);
        //}

        ////Send: ������ ID PW.
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

        ////Receive: �����κ��� �α��� ��ȿ�� �˻�.
        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {
            await UIManager.Show<UILobby>();
        }
    }

    public void TrySetTask(bool isSuccess)
    {
        if (sourceTcs.TrySetResult(isSuccess))
        {
            Debug.Log("ȸ������ ����");
        }
    }

    

    //Inspector: �������� �ǳ� Ű��
    public async void OnQuitBtn()
    {
        await UIManager.Show<UIQuit>();
    }
    #endregion

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
