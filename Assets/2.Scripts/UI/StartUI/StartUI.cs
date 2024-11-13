using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : UIBase
{
    [SerializeField]
    private Sprite logo;
    [SerializeField]
    private Button[] btnStart;

    private string targetScene = "IceBoard";

    private string aaa = "temp_super_convergence";
    private void Start()
    {
        InitBtn();
        LoadLogoImage();
    }

    private async void LoadLogoImage()
    {
        logo = await ResourceManager.Instance.LoadAsset<Sprite>(aaa, eAddressableType.Texture);
    }

    private void InitBtn()
    {
        btnStart[0].onClick.AddListener(LoadTargetScene);
        btnStart[1].onClick.AddListener(OpenSettingUI);
        btnStart[2].onClick.AddListener(QuitProgram);
    }

    private void LoadTargetScene()
    {
        SceneLoad.LoadSceneByName(targetScene);
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
}
