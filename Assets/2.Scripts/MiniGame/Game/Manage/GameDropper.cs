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

        /*Reset Players - Socket 필요*/
        //if (param.Length > 0 && param[0] is S2C_IceMiniGameReadyNotification response)
        //{
        //    ResetPlayers(response.Players);
        //}
        //else
        //{
        //    Debug.LogError("param parsing error : startPlayers");
        //}
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
        
        //TODO : 나에 맞는 inputsystem 정의
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
        {
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            miniToken.transform.localPosition = SocketManager.ToVector3(p.Position);//현재 위치
            miniToken.MiniData.nextPos = SocketManager.ToVector3(p.Position); //다음 위치
            miniToken.MiniData.rotY = p.Rotation; //현재 회전값
        }
    }
    #endregion

    #region 인게임 이벤트
    private void SetSlotPosition(int slot)
    {

    }

    #endregion
}