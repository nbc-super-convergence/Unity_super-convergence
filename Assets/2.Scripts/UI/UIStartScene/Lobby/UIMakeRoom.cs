using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIMakeRoom : UIBase
{
    [SerializeField] private TMP_InputField RoomNameInput;

    public void OnBackBtn()
    {
        UIManager.Hide<UIMakeRoom>();
    }

    public async void OnApplyBtn()
    {
        UIManager.Hide<UIMakeRoom>();
        //Send Make Room
        //πÊ ¿Ã∏ß: RoomNameInput.text 
        await UIManager.Show<UIRoom>();
        UIManager.Hide<UILobby>();
    }
}
