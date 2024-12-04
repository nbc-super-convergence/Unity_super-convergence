
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
public class InsuDebugger : Singleton<InsuDebugger>
{
    public bool isSingle;
   
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) 
        { JumpBoardScene(); }
        if (Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) 
        { JumpRoomUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha8) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { UIManager.Get<UIRoom>().GameStart(); }
        if (Input.GetKeyDown(KeyCode.Alpha7) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { JumpExecuteCourtshipDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha6) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { StartDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha5) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { UIManager.Get<UICommandBoardHandler>().Next(); }
    }

    private void JumpBoardScene()
    {
        UserInfo debugInfo = new();
        debugInfo.SetSessionId("debugSessionId");
        debugInfo.SetSessionId("debugNickName");
        debugInfo.SetSessionId("debugSessionId");

        GameManager.Instance.myInfo = debugInfo;

        UIManager.Get<UIRoom>().GameStart();
    }

    private async void JumpRoomUI()
    {
        UserInfo debugInfo = new();
        debugInfo.SetSessionId("debugSessionId");
        debugInfo.SetSessionId("debugNickName");
        debugInfo.SetSessionId("debugSessionId");

        GameManager.Instance.myInfo = debugInfo;

        RoomData roomData = new RoomData()
        {
            RoomId = "debugId",
            OwnerId = GameManager.Instance.myInfo.SessionId,
            RoomName = "DebugRoom",
            LobbyId = "debugLobbyId",
            State = RoomStateType.Prepare,
            MaxUser = 4,
            
        };
        await UIManager.Show<UIRoom>(roomData);
    }

    private void JumpExecuteCourtshipDance()
    {
        GameManager.Instance.myInfo.SetSessionId("debugInsu");
        SceneManager.LoadScene(3);
    }
    private void StartDance()
    {        
        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(new List<Player>()
        {
            new Player(){ SessionId = GameManager.Instance.myInfo.SessionId }
        });
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GameStart();
    }
}
#endif