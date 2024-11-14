
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private UIBase startUI;

    private void Start()
    {
        InitBtn();
    }
        
    private void InitBtn()
    {
        buttons[0].onClick.AddListener(Back);
        buttons[1].onClick.AddListener(Apply);
    }

    private void Back()
    {
        gameObject.SetActive(false);
        startUI.SetActive(true);
    }
    private void Apply()
    {
        Debug.Log($"Apply is Not Ready");
    }

}
