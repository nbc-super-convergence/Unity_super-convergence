using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GamePacket;

public class UIStart : UIBase
{
    public Image logoImg;
    [SerializeField] private Button[] btnStart;
    [SerializeField] private TextMeshProUGUI currentVersion;

    private TMP_InputField inputFieldPlayerID;
    private string strPlayerID;
    [SerializeField] private TextMeshProUGUI currentPlayerID;
    private int playerID;

    private string targetScene = "IceBoard";
    private string logoKey = "temp_super_convergence";
    private string curVersion = "0.000";

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        InitBtn();
        Init();
        GetCurrentVersion();

        inputFieldPlayerID = GetComponentInChildren<TMP_InputField>();
    }

    private async void Init()
    {
        Sprite img = await ResourceManager.Instance.LoadAsset<Sprite>(logoKey, eAddressableType.Texture);
        logoImg.sprite = img;

        await UIManager.Show<UILogin>();
    }

    private void InitBtn()
    {
        btnStart[0].onClick.AddListener(LoadTargetScene);
        btnStart[1].onClick.AddListener(OpenSettingUI);
        btnStart[2].onClick.AddListener(QuitProgram);
    }

    private void LoadTargetScene()
    {
        if (!(1 <= playerID && playerID <= 4))
        {
            Debug.Log("올바른 PlayerID를 입력하시고 Apply 버튼을 눌러주세요.");
            return;
        }

        if (IsSceneInBuild(targetScene))
        {
            SocketManager.Instance.Init();
            SendPacketIceJoinRequest();

            SceneManager.LoadScene(targetScene);
        }
    }

    /// <summary>
    /// 패킷
    /// </summary>
    private void SendPacketIceJoinRequest()
    {
        GamePacket packet = new();
        packet.IceJoinRequest = new()
        {
            PlayerId = playerID
        };
        SocketManager.Instance.OnSend(packet);
    }

    private async void OpenSettingUI()
    {       
        UIManager.Hide<UIStart>();
        await UIManager.Show<UISetting>();        
    }
    private void QuitProgram()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GetCurrentVersion()
    {
        //TODO:: 서버에서 버전정보를 가져오거나 출력하는 코드
        string version = $"Current Version: {curVersion}";
        currentVersion.text = version;
    }

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

    /// <summary>
    /// 인스펙터에서 직접 설정.
    /// </summary>
    public void ButtonApplyPlayerID()
    {
        strPlayerID = inputFieldPlayerID.text;
        if (strPlayerID.Length == 0)
        {
            Debug.Log($"PlayerID 입력이 없습니다.");
            return;
        }
        else if (strPlayerID != "1" && strPlayerID != "2" && strPlayerID != "3" && strPlayerID != "4")
        {
            Debug.Log($"1 ~ 4의 숫자를 입력해주세요.");
            return;
        }

        playerID = int.Parse(strPlayerID);

        currentPlayerID.text = playerID.ToString();
    }
}