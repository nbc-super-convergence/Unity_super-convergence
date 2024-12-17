using Google.Protobuf.Collections;
using UnityEngine;

public class GameIceSlider : IGame
{
    private GameIceSliderData gameData;
    private UIMinigameIce ingameUI;

    #region IGame
    public async void Init(params object[] param)
    {
        gameData = new GameIceSliderData();
        gameData.Init();
        MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameIceSlider>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap<MapGameIceSlider>();
        SetBGM();

        if (param.Length > 0 && param[0] is S2C_IceMiniGameReadyNotification response)
        {
            ResetPlayers(response.Players);
        }
        else
        {
            Debug.LogError("startPlayers 자료형 전달 과정에서 문제 발생");
        }
    }
    public async void GameStart(params object[] param)
    {
        ingameUI = await UIManager.Show<UIMinigameIce>(gameData);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }
    #endregion

    #region 초기화
    private void SetBGM()
    {
        SoundManager.Instance.PlayBGM(BGMType.Ice);
    }

    private void ResetPlayers(RepeatedField<S2C_IceMiniGameReadyNotification.Types.startPlayers> players)
    {
        foreach (var p in players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            miniToken.transform.localPosition = SocketManager.ToVector3(p.Position);
            miniToken.MiniData.nextPos = SocketManager.ToVector3(p.Position);
            miniToken.MiniData.rotY = p.Rotation;

            miniToken.MiniData.PlayerSpeed = 15f;
        }
    }
    #endregion

    #region 인게임 이벤트
    public void GiveDamage(string sessionId, int dmg, bool isMe = false)
    {
        int idx = GameManager.Instance.SessionDic[sessionId].Color;
        gameData.playerHps[idx] -= dmg;
        ingameUI.ChangeHPUI();
    }

    public void PlayerDeath(string sessionId)
    {
        int idx = GameManager.Instance.SessionDic[sessionId].Color;
        gameData.playerHps[idx] = 0;
        ingameUI.ChangeHPUI();

        MiniToken token = MinigameManager.Instance.GetMiniToken(sessionId);
        if (sessionId == MinigameManager.Instance.mySessonId)
        {
            token.DisableMyToken();
        }
        token.DisableMiniToken();
    }

    public async void MapChangeEvent()
    {
        gameData.phase++; //맵 변경 단계
        var map = await MinigameManager.Instance.GetMap<MapGameIceSlider>();
        map.MapDecreaseEvent(gameData.phase);
    }
    #endregion

    public void DisableUI()
    {
        UIManager.Hide<UIMinigameIce>();
    }
}