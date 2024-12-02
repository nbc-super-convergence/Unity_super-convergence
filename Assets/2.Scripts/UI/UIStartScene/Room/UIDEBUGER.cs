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
        if (Input.GetKey(KeyCode.Alpha0)) { JumpBoardScene(); }
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
}