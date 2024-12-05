using System.Collections.Generic;

// 임시 클래스
public class Player
{
    public string SessionId;
}

// 이 클래스는 미니게임매니저에 올라가게 됨.
public class GameCourtshipDance : IGame
{
    public int boardCount = 13;

    public UICommandBoardHandler commandBoardHandler;

    private CommandGenerator commandGenerator;
    private Queue<Queue<BubbleInfo>> commandInfoPool;
    private List<Player> players;    // TODO:: 패킷정보에 맞게 고치기



    public GameCourtshipDance()
    {
    }

    /// <summary>
    /// MinigameManager의 SetMiniGame에서 처음 실행함.
    /// param (== response) 안에 서버로부터 받은 players 참가유저 정보가 들어있음.
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
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

        ResetPlayers(players);

        if (GameManager.Instance.myInfo.SessionId == players[0].SessionId)
        {
            commandGenerator = new CommandGenerator();
            commandInfoPool = commandGenerator.GenerateBoardPool(boardCount);

            commandGenerator.SetBoardPoolColor(commandInfoPool, players);
        } // 커맨드보드 제작과 전송완료대기 리퀘스트 패킷, 그 응답,알림 패킷



        //MinigameManager.Instance.CurMap = await ResourceManager.Instance.LoadAsset<MapGameCourtshipDance>($"Map{MinigameManager.GameType}", eAddressableType.Prefab);
        //MinigameManager.Instance.MakeMap();

        //if (param.Length > 0 && param[0] is S2C_IceMiniGameReadyNotification response)
        //{
        //    ResetPlayers(response.Players);
        //}
        //else
        //{
        //    Debug.LogError("startPlayers 자료형 전달 과정에서 문제 발생");
        //}
    }

    /// <summary>
    /// S2C게임시작알림 서버의 알림에 따라 실행. 진짜 게임 시작.
    /// </summary>
    public async void GameStart(params object[] param)
    {
        var commandBoardHandler = await UIManager.Show<UICommandBoardHandler>();
        commandBoardHandler.Make(players.Count);
        //MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }

    public Queue<Queue<BubbleInfo>> GetCommandInfoPool()
    {
        return commandInfoPool;
    }



    #region 초기화
    // 팀 가르기
    

    private void ResetPlayers(List<Player> players) // 매개변수 바뀔 수 있음.
    {
        var map = MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
        int num = 0;
        foreach (var p in players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            //miniToken.transform.localPosition = SocketManager.ToVector3(p.Position);
            if (players.Count != 4)
            {
                //개인전 세팅. 팀가르기 없이 차례대로 배치하기. 커맨드보드를 4개 생성.
                miniToken.transform.localPosition = map.spawnPosition[num].position;
                miniToken.MiniData.nextPos = map.spawnPosition[num].position;
                miniToken.MiniData.rotY = map.spawnPosition[num].rotation.y;
            }
            else
            {
                // 4명이면 2:2 팀전 세팅

            }
            num++;
        }
    }
    #endregion

    #region 소켓
    // S2C GameStartNoti
    public void GameStartNoti(GamePacket packet)
    {
        var response = packet.GameStartNotification;
        MinigameManager.Instance.SetMiniGame<GameCourtshipDance>(response);
    }
    #endregion

#if UNITY_EDITOR

#endif
}