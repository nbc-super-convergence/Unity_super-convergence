using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

//// 임시 클래스
//public class PlayerInfo
//{
//    public string SessionId;
//    public int TeamId;
//}

// 이 클래스는 미니게임매니저에 올라가게 됨.
public class GameCourtshipDance : IGame
{
    private UICourtshipDance ingameUI;
    public UICourtshipDance commandBoardHandler;
    public CourtshipDanceData gameData;

    private CommandGenerator commandGenerator;
    public Dictionary<string, Queue<Queue<BubbleInfo>>> commandPoolDic;
    private List<PlayerInfo> players = new();   
    private TaskCompletionSource<bool> sourceTcs;

    public GameCourtshipDance()
    {
    }

    /// <summary>
    /// MinigameManager의 SetMiniGame에서 처음 실행함.
    /// param (== response) 안에 서버로부터 받은 players 참가유저 정보가 들어있음.
    /// </summary>
    /// <param name="param"></param>
    public async void Init(params object[] param)
    {
        gameData = new CourtshipDanceData();
        gameData.Init();
        var commandBoardHandler = await UIManager.Show<UICourtshipDance>(gameData);
        MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameCourtshipDance>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMapDance();
        if (param[0] is S2C_DanceMiniGameReadyNotification response)
        {
            foreach (var p in response.Players)
            {
                this.players.Add(p);
            }
        }
        else if (param[0] is RepeatedField<PlayerInfo> players)
        {
            foreach ( var p in players)
            {
                this.players.Add(p);
            }
        }


        // 토큰 배치 및 세팅하기
        ResetPlayers(players);

        if (GameManager.Instance.myInfo.SessionId == players[0].SessionId)
        {
            commandGenerator = new CommandGenerator();
            commandGenerator.InitFFA(players);
            commandPoolDic = commandGenerator.GetPlayerPoolDic();
        }
        // 커맨드보드 제작과 전송완료대기 리퀘스트 패킷, 그 응답,알림 패킷
        /* 405 */
        sourceTcs = new();
        if (commandGenerator != null)
        {
            List<DancePool> sp = CommandGenerator.ConvertToDancePools(commandPoolDic);

            GamePacket packet = new();

            packet.DanceTableCreateRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId,
            };
            packet.DanceTableCreateRequest.DancePools.Add(sp);
            //sourceTcs = new();
            SocketManager.Instance.OnSend(packet);
        }
        else
        {
            await sourceTcs.Task;
            // TODO:: 이쯤에 로딩 완료 표시하는 기능 넣기.
        }

        commandBoardHandler.MakeCommandBoard(players);
        
    }

    public void SetCommandPoolDic(RepeatedField<DancePool> dancePools)
    {
        commandPoolDic = CommandGenerator.ConvertToPlayerPoolDic(dancePools);
    }

    public void TrySetTask(bool isSuccess)
    {
        bool b = sourceTcs.TrySetResult(isSuccess);
    }


    /// <summary>
    /// S2C게임시작알림 서버의 알림에 따라 실행. 진짜 게임 시작.
    /// </summary>
    public async void GameStart(params object[] param)
    {
        if (param[0] is long startTime)
        {
            ingameUI = await UIManager.Show<UICourtshipDance>(gameData, startTime);
        }
        MinigameManager.Instance.GetMyToken().EnableInputSystem(eGameType.GameCourtshipDance);
    }

    public void BeforeGameEnd()
    {
        var map = MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
        foreach (var p in players)
        {
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            map.TokenReset(miniToken);
        }
        UIManager.Hide<UICourtshipDance>();
    }
    
    #region 초기화
    // 팀 가르기

    /// <summary>
    /// 토큰의 위치 지정과 애니메이터 교체를 수행.
    /// 입력 교체
    /// </summary>
    /// <param name="players"></param>
    private void ResetPlayers(List<PlayerInfo> players) // 매개변수 바뀔 수 있음.
    {
        var map = MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
        int num = 0;
        foreach (var p in players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            if (true)
            {
                //개인전 세팅. 팀가르기 없이 차례대로 배치하기. 커맨드보드를 4개 생성.
                miniToken.transform.position = map.spawnPosition[num].position;
                miniToken.transform.rotation = map.spawnPosition[num].rotation;
                miniToken.MiniData.nextPos = map.spawnPosition[num].position;
                miniToken.MiniData.rotY = map.spawnPosition[num].rotation.y;
                miniToken.InputHandler.ChangeActionMap("SimpleInput");
                map.TokenInit(miniToken);
            }
            else
            {
                // 4명이면 2:2 팀전 세팅

            }
            num++;
        }
    }
    #endregion


    public int GetPlayerTeam(string sessionId)
    {
        foreach (var p in players)
        {
            if(p.SessionId == sessionId)
            {
                return p.TeamNumber;                
            }            
        }
        return -1;
    }
    public string GetPlayerSessionId(int teamNumber)
    {
        foreach (var p in players)
        {
            if (p.TeamNumber == teamNumber)
            {
                return p.SessionId;
            }
        }
        return null;
    }
}