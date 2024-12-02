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
        if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) 
        { JumpBoardScene(); }
        if (Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) 
        { JumpRoomUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha8) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { UIManager.Get<UIRoom>().GameStart(); }
    }

    private void JumpBoardScene()
    {
        UserInfo debugInfo = new();
        debugInfo.userData.SessionId = "debugSessionId";
        debugInfo.userData.Nickname = "debugNickName";
        debugInfo.SetSessionId("debugSessionId");
        debugInfo.SetUuid("debugUuid");

        GameManager.Instance.myInfo = debugInfo;

        UIManager.Get<UIRoom>().GameStart();
    }

    private async void JumpRoomUI()
    {
        UserInfo debugInfo = new();
        debugInfo.userData.SessionId = "debugSessionId";
        debugInfo.userData.Nickname = "debugNickName";
        debugInfo.SetSessionId("debugSessionId");
        debugInfo.SetUuid("debugUuid");

        GameManager.Instance.myInfo = debugInfo;

        RoomData roomData = new RoomData()
        {
            RoomId = "debugId",
            OwnerId = GameManager.Instance.myInfo.sessionId,
            RoomName = "DebugRoom",
            LobbyId = "debugLobbyId",
            State = RoomStateType.Prepare,
            MaxUser = 4,
            
        };
        

        await UIManager.Show<UIRoom>(roomData);
    }
}