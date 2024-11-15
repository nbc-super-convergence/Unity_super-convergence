using UnityEngine;
using UnityEngine.UI;

public class UILobby : UIBase
{
    [SerializeField]
    private Button[] buttons;

    private void Start()
    {
        InitBtn();
    }

    private void InitBtn()
    {
        buttons[0].onClick.AddListener(Back);
        buttons[1].onClick.AddListener(Apply);
    }

    private async void Back()
    {
        UIManager.Hide<UIMakeRoom>();
        await UIManager.Show<UILobby>();
    }
    private async void Apply()
    {
        //Debug.Log($"Apply is Not Ready");

        UIManager.Hide<UILobby>();
        await UIManager.Show<UIMakeRoom>();
    }
}