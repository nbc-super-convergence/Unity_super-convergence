using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStart : UIBase
{
    public Image logoImg;
    [SerializeField]
    private Button[] btnStart;
    [SerializeField]
    private TextMeshProUGUI currentVersion;
    [SerializeField]
    private bool isFirst = true;

    [SerializeField]
    private TMP_InputField inputFieldPlayerID;
    [SerializeField] private string strPlayerID;
    [SerializeField] private TextMeshProUGUI currentPlayerID;
    public int playerID;

    private string targetScene = "IceBoard";
    private string logoKey = "temp_super_convergence";
    private string curVersion = "0.000";

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        InitBtn();
        LoadLogoImage();
        GetCurrentVersion();

        inputFieldPlayerID = GetComponentInChildren<TMP_InputField>();
    }

    private async void LoadLogoImage()
    {
        Sprite img = await ResourceManager.Instance.LoadAsset<Sprite>(logoKey, eAddressableType.Texture);
        logoImg.sprite = img;
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
            Debug.Log("�ùٸ� PlayerID�� �Է��Ͻð� Apply ��ư�� �����ּ���.");
            return;
        }

        if (IsSceneInBuild(targetScene))
        {
            SocketManager.Instance.Init();
            //SocketManager.Instance.OnSend()

            SceneManager.LoadScene(targetScene);
        }
    }
    private async void OpenSettingUI()
    {       
        UIManager.Hide<UIStart>();
        await UIManager.Show<UISetting>();

        if (isFirst)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
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
        //TODO:: �������� ���������� �������ų� ����ϴ� �ڵ�
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
    /// �ν����Ϳ��� ���� ����.
    /// </summary>
    public void ButtonApplyPlayerID()
    {
        strPlayerID = inputFieldPlayerID.text;
        if (strPlayerID.Length == 0)
        {
            Debug.Log($"PlayerID �Է��� �����ϴ�.");
            return;
        }
        else if (strPlayerID != "1" && strPlayerID != "2" && strPlayerID != "3" && strPlayerID != "4")
        {
            Debug.Log($"1 ~ 4�� ���ڸ� �Է����ּ���.");
            return;
        }

        playerID = int.Parse(strPlayerID);

        currentPlayerID.text = playerID.ToString();
    }
}