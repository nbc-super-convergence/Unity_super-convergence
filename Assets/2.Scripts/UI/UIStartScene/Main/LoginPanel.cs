using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public static bool isSuccessLogin = false;

    [SerializeField] private TMP_InputField IDInput;
    [SerializeField] private TMP_InputField PasswardInput;

    [SerializeField] private TextMeshProUGUI versionTxt;

    private string targetScene = "IceBoard";

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
    public void OnLoginBtn()
    {
        //�׽�Ʈ �ڵ�
        if (IsSceneInBuild(targetScene))
        {
            SocketManager.Instance.Init();
            SendPacketIceJoinRequest();
            SceneManager.LoadScene(targetScene);
        }

        ////Send: ������ ID PW.
        //string id = IDInput.text;
        //string passward = PasswardInput.text;
        ////Receive: �����κ��� �α��� ��ȿ�� �˻�.

        //if (isSuccessLogin)
        //{
        //    await UIManager.Show<UILobby>();
        //}
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
