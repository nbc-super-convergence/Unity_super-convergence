using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogout : UIBase
{
    private Button btnLogout;


    private void Start()
    {
        btnLogout = GetComponentInChildren<Button>();
        btnLogout.onClick.AddListener(ButtonLogout);
    }

    private async void ButtonLogout()
    {
        //TODO:: �α׾ƿ� ��Ŷ ������

        UIManager.Hide<UILogout>();
        await UIManager.Show<UILogin>();
    }
}
