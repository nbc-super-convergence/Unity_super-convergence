using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDEBUGER : MonoBehaviour
{
    public Button[] button;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
                     
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) { JumpBoardScene(); }
        if (Input.GetKey(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) {}
    }

    private void JumpBoardScene()
    {
        UserInfo debugInfo = new();
        debugInfo.userData.SessionId = "debugSessionId";
        debugInfo.userData.Nickname = "debugNickName";
        debugInfo.SetSessionId("debugSessionId");
        debugInfo.SetUuid("debugUuid");

        GameManager.Instance.myInfo = debugInfo;

        SceneManager.LoadScene(2);
    }

    private void JumpRoomUI()
    {
        UserInfo debugInfo = new();
        debugInfo.userData.SessionId = "debugSessionId";
        debugInfo.userData.Nickname = "debugNickName";
        debugInfo.SetSessionId("debugSessionId");
        debugInfo.SetUuid("debugUuid");

        GameManager.Instance.myInfo = debugInfo;

        UIManager.Show<UIRoom>();
    }
}