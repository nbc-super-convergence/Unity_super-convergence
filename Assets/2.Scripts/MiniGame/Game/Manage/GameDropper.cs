using Google.Protobuf.Collections;
using UnityEngine;
using static S2C_IceMiniGameReadyNotification.Types;

public class GameDropper : IGame
{
    private GameDropperData gameData;
    private UIMinigameDropper ingameUI;

    #region IGame
    public async void Init(params object[] param)
    {
        gameData = new GameDropperData();
        gameData.Init();
        MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameDropper>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap();
        SetBGM();

        //if (param.Length > 0 && param[0] is S2C_IceMiniGameReadyNotification response)
        //{
        //    ResetPlayers(response.Players);
        //}
        //else
        //{
        //    Debug.LogError("startPlayers 자료형 전달 과정에서 문제 발생");
        //}
    }

    public async void GameStart()
    {
        ingameUI = await UIManager.Show<UIMinigameDropper>(gameData);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }
    #endregion

    #region 초기화
    private void SetBGM()
    {

    }

    private void ResetPlayers(RepeatedField<startPlayers> players)
    {
        foreach (var p in players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            miniToken.transform.localPosition = SocketManager.ToVector3(p.Position);
            miniToken.MiniData.nextPos = SocketManager.ToVector3(p.Position);
            miniToken.MiniData.rotY = p.Rotation;
        }
    }
    #endregion

    #region 인게임 이벤트
    //public void GiveDamage(string sessionId, int dmg, bool isMe = false)
    //{
    //    int idx = GameManager.Instance.SessionDic[sessionId].Color;
    //    gameData.playerHps[idx] -= dmg;
    //    ingameUI.ChangeHPUI();

    //    /*서버 없을 때 임시 로직*/
    //    if (gameData.playerHps[idx] == 0)
    //    {
    //        MinigameManager.Instance.GetMiniGame<GameIceSlider>()
    //        .PlayerDeath(sessionId);
    //    }
    //}

    //public void PlayerDeath(string sessionId)
    //{
    //    MiniToken token = MinigameManager.Instance.GetMiniToken(sessionId);
    //    if (sessionId == MinigameManager.Instance.mySessonId)
    //    {
    //        token.DisableMyToken();
    //    }
    //    token.DisableMiniToken();
    //}

    //public void MapChangeEvent()
    //{
    //    gameData.phase++; //맵 변경 단계
    //    MinigameManager.Instance.GetMap<MapGameIceSlider>()
    //        .MapDecreaseEvent(gameData.phase);
    //}
    #endregion
}