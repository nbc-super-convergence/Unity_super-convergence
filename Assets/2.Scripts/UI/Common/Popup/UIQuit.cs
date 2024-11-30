using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIQuit : UIBase
{
    //Inspector: 돌아가기
    public void OnBackBtn()
    {
        UIManager.Hide<UIQuit>();
    }

    //Inspector: 진짜 게임종료
    public void OnQuitBtn()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
