using UnityEditor;

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
