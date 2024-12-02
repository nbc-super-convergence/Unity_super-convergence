using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIQuit : UIBase
{
    //Inspector: ���ư���
    public void OnBackBtn()
    {
        UIManager.Hide<UIQuit>();
    }

    //Inspector: ��¥ ��������
    public void OnQuitBtn()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
