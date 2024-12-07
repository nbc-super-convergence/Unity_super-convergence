
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
        if (Input.GetKeyDown(KeyCode.Alpha7) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { JumpExecuteCourtshipDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha6) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { StartDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha5) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { UIManager.Get<UICommandBoardHandler>().Next(GameManager.Instance.myInfo.SessionId); }
    }

    private void OpenDance()
    {
        // 보드에서 미니게임 입장.
        SceneManager.LoadScene(3);
    }

    private void JumpExecuteCourtshipDance()
    {
        GameManager.Instance.myInfo.SetSessionId("debugInsu");
        SceneManager.LoadScene(3);
    }
    private void StartDance()
    {        
        Player debugPlayer0 = new Player() { SessionId = GameManager.Instance.myInfo.SessionId };
        List<Player> debugPlayers = new List<Player>
        {
            debugPlayer0
        };

        GameManager.Instance.AddNewPlayer(debugPlayer0.SessionId, "DebugInsu", 0, 0);

        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(debugPlayers);
        MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GameStart();
    }
}
#endif