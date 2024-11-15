
using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIBase
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
        UIManager.Hide<UISetting>();
        await UIManager.Show<UIStart>();
    }
    private void Apply()
    {
        Debug.Log($"Apply is Not Ready");
    }

}
