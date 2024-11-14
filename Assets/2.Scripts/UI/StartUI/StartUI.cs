using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : UIBase
{
    public Image logoImg;
    [SerializeField]
    private Button[] btnStart;
    [SerializeField]
    private TextMeshProUGUI currentVersion;
    
    [SerializeField]
    private UIBase settingUI;

    private string targetScene = "IceBoard";
    private string logoKey = "temp_super_convergence";
    private string curVersion = "0.000";

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        InitBtn();
        LoadLogoImage();
        GetCurrentVersion();
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
        if (SceneManager.GetSceneByName(targetScene).IsValid())
        {
            SceneLoad.LoadSceneByName(targetScene);
        }
        else
        {
            Debug.Log($"�ε��Ϸ��� {targetScene}�� �������� �ʽ��ϴ�.");
        }
    }
    private void OpenSettingUI()
    {
        Debug.Log($"Setting is Not Ready");
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
}
