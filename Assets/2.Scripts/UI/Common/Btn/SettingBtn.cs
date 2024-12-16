using UnityEngine;

public class SettingBtn : MonoBehaviour
{
    //Inspector: 설정 버튼
    public async void OnBtnSetting()
    {
        await UIManager.Show<UISetting>();
    }
}