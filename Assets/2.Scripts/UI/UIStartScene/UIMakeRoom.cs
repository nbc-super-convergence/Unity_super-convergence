using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIMakeRoom : UIBase
{
    [SerializeField] private InputField inputFieldRoomName;
    [SerializeField] private InputField inputFieldPassword;
    private StringBuilder roomName = new StringBuilder();
    private StringBuilder roomPassword = new StringBuilder();

    [SerializeField]
    private Button[] buttons;

    [SerializeField]
    private ToggleGroup maxPlayerToggleGroup;

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

    private void Apply()
    {
        Debug.Log($"Apply is Not Ready");
    }

    private void GetRoomName()
    {
        roomName.Append(inputFieldRoomName.text);
    }
    private void GetPassword()
    {
        roomName.Append(inputFieldPassword.text);
    }
    private int GetToggleValue(ToggleGroup toggleGroup)
    {
        foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                return int.Parse(toggle.GetComponentInChildren<Text>().text);
            }
        }
        return 4;
    }
}
