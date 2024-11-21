
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

    private void Back()
    {
        UIManager.Hide<UISetting>();
    }

    private void Apply()
    {
        Debug.Log($"Apply is Not Ready");
    }

}
