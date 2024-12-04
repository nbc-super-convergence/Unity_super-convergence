using System.Collections.Generic;
using Google.Protobuf.Collections;
using UnityEngine.Playables;
using static S2C_IceMiniGameReadyNotification.Types;

public class Player
{
    public string SessionId;
}

public class GameCourtshipDance : IGame
{
    private CommandGenerator commandGenerator;
    private Queue<Queue<BubbleInfo>> commandInfoPool;
    private List<Player> players;    // TODO:: 패킷정보에 맞게 고치기

    /// <summary>
    /// MinigameManager의 SetMiniGame에서 처음 실행함.
    /// param (== response) 안에 서버로부터 받은 players 참가유저 정보가 들어있음.
    /// </summary>
    /// <param name="param"></param>
    public async void Init(params object[] param)
    {
        // S2C_IceMiniGameReadyNotification은 미니게임 ReadyPanel때 나오는 정보
        // ReadyPanel을 띄움과 동시에 MinigameManager에서 데이터 설정, 맵 설정, BGM 설정을 한다.        
        S2C_IceMiniGameReadyNotification response;
        if (param[0] is S2C_IceMiniGameReadyNotification item)
        {
            response = item;
            //this.players = item.Players;
        }
        else if (param[0] is List<Player> players)
        {
            this.players = players;
        }

        if (GameManager.Instance.myInfo.SessionId == players[0].SessionId)
        {
            commandGenerator = new CommandGenerator();
            commandInfoPool = commandGenerator.GenerateBoardPool(players.Count);

            // 색 정보등 입력.
            commandGenerator.SetBoardPoolColor(commandInfoPool, players);
            // 커맨드보드에 info 적용하기.
        }

        MinigameManager.Instance.CurMap = await ResourceManager.Instance.LoadAsset<MapGameCourtshipDance>($"Map{MinigameManager.GameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap();

        //if (param.Length > 0 && param[0] is S2C_IceMiniGameReadyNotification response)
        //{
        //    ResetPlayers(response.Players);
        //}
        //else
        //{
        //    Debug.LogError("startPlayers 자료형 전달 과정에서 문제 발생");
        //}
    }
    public void GameEnd(Dictionary<string, int> ranks)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// S2C게임시작알림 서버의 알림에 따라 실행.
    /// </summary>
    public void GameStart()
    {
        //ingameUI = await UIManager.Show<UIMinigameIce>(gameData);
        //MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }

    public Queue<Queue<BubbleInfo>> GetCommandInfoPool()
    {
        return commandInfoPool;
    }

    #region 소켓

    #endregion

#if UNITY_EDITOR

#endif
}