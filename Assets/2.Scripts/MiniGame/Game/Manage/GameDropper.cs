using Google.Protobuf.Collections;
using UnityEngine;

public class GameDropper : IGame
{
    public GameDropperData gameData { get; private set; }
    private UIMinigameDropper ingameUI;

    #region IGame
    public async void Init(params object[] param)
    {
        gameData = new GameDropperData();
        gameData.Init();
        MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameDropper>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap<MapGameDropper>();
        SetBGM();

        /*Reset Players*/
        if (param.Length > 0 && param[0] is S2C_DropMiniGameReadyNotification response)
        {
            ResetPlayers(response.Players);
        }
        else
        {
            Debug.LogError("param parsing error : startPlayers");
        }
    }

    public async void GameStart(params object[] param)
    {
        if (param.Length == 1)
        {
            if (param[0] is long startTime)
            {
                ingameUI = await UIManager.Show<UIMinigameDropper>(gameData, startTime);
            }
            else
            {
                Debug.LogError("param parsing error : startTime");
                return;
            }
        }
        else
        {
            Debug.LogError("param length error");
            return;
        }
    }
    #endregion

    #region 초기화
    private void SetBGM()
    {
        SoundManager.Instance.PlayBGM(BGMType.Dropper);
    }

    private void ResetPlayers(RepeatedField<S2C_DropMiniGameReadyNotification.Types.startPlayers> players)
    {
        foreach (var p in players)
        {
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();

            Vector3 initPos = GetSlotPosition(p.Slot);
            if (p.SessionId == MinigameManager.Instance.mySessonId)
            {
                gameData.curSlot = p.Slot;
            }

            miniToken.transform.localPosition = initPos;
            miniToken.MiniData.nextPos = initPos;
            miniToken.MiniData.rotY = p.Rotation;
            miniToken.MiniData.CurState = State.Idle;

            miniToken.MiniData.PlayerSpeed = 5f;
        }
    }
    #endregion

    #region 메인함수
    public void ReceiveMove(string sessionId, int slot, float rotation, State state)
    {
        MiniToken miniToken = MinigameManager.Instance.GetMiniToken(sessionId);
        miniToken.MiniData.nextPos = GetSlotPosition(slot, miniToken.transform.position.y);
        miniToken.MiniData.rotY = rotation;

        if (sessionId == MinigameManager.Instance.mySessonId)
        {
            gameData.curSlot = slot;
        }
    }

    public void PlayerDeath(string sessionId)
    {
        MinigameManager.Instance.GetMiniToken(sessionId).DisableMiniToken();
    }

    public void DisableUI()
    {
        UIManager.Hide<UIMinigameDropper>();    
    }
    #endregion

    #region 보조함수
    private Vector3 GetSlotPosition(int slot, float yPos = 60.00398f)
    {
        Vector3 pos;
        float xPos = 0, zPos = 0;

        //xPos 
        switch (slot)
        {
            case 0:
            case 1:
            case 2:
                zPos = -2.1f;
                break;
            case 3:
            case 4:
            case 5:
                zPos = 0f;
                break;
            case 6:
            case 7:
            case 8:
                zPos = 2.1f;
                break;
        }

        //xPos
        switch (slot)
        {
            case 0:
            case 3:
            case 6:
                xPos = 2.1f;
                break;
            case 1:
            case 4:
            case 7:
                xPos = 0f;
                break;
            case 2:
            case 5:
            case 8:
                xPos = -2.1f;
                break;
        }

        pos = new Vector3(xPos, yPos, zPos);
        return pos;
    }
    #endregion
}