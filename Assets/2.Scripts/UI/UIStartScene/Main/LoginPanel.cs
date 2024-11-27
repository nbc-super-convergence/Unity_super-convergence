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
    //Inspector: 회원가입 판넬 키기
    public async void OnRegisterBtn()
    {
        await UIManager.Show<UIRegister>();
    }

    //Inspector: 로그인 후 로비로 들어가기
    public void OnLoginBtn()
    {
        //테스트 코드
        if (IsSceneInBuild(targetScene))
        {
            SocketManager.Instance.Init();
            SendPacketIceJoinRequest();
            SceneManager.LoadScene(targetScene);
        }

        ////Send: 서버로 ID PW.
        //string id = IDInput.text;
        //string passward = PasswardInput.text;
        ////Receive: 서버로부터 로그인 유효성 검사.

        //if (isSuccessLogin)
        //{
        //    await UIManager.Show<UILobby>();
        //}
    }

    //Inspector: 게임종료 판넬 키기
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
