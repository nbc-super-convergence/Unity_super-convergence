using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingBtn : MonoBehaviour
{
    //Inspector: ���� ��ư
    public async void OnBtnSetting()
    {
        await UIManager.Show<UISetting>();
    }
}
