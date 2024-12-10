
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
        { OpenDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            Setminigame(); 
            MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GameStart();
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            UIManager.Get<UICourtshipDance>().Next("Session1");
            UIManager.Get<UICourtshipDance>().Next("Session2");
            UIManager.Get<UICourtshipDance>().Next("Session3");
            UIManager.Get<UICourtshipDance>().Next("Session4");
        }


        if (Input.GetKeyDown(KeyCode.Alpha7) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { JumpExecuteCourtshipDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha6) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { StartDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha5) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { UIManager.Get<UICourtshipDance>().Next(GameManager.Instance.myInfo.SessionId); }
    }

    private void OpenDance()
    {
        // 보드에서 미니게임 입장.
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }

    private void Setminigame()
    {
        var a = GameManager.Instance.SessionDic;
        List<PlayerInfo> list = new List<PlayerInfo>();

        foreach (var p in a)
        {
            var player = new PlayerInfo { SessionId = p.Value.SessionId , TeamNumber = 0 };
            list.Add(player);
        }

        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(list);
    }
    

    private void JumpExecuteCourtshipDance()
    {
        GameManager.Instance.myInfo.SetSessionId("debugInsu");
        SceneManager.LoadScene(3);
    }
    private void StartDance()
    {        
        PlayerInfo debugPlayer0 = new PlayerInfo() { SessionId = GameManager.Instance.myInfo.SessionId };
        List<PlayerInfo> debugPlayers = new List<PlayerInfo>
        {
            debugPlayer0
        };

        GameManager.Instance.AddNewPlayer(debugPlayer0.SessionId, "DebugInsu", 0, 0);

        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(debugPlayers);
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GameStart();
    }
}
#endif