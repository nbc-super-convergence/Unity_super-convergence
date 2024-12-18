using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameCourtshipDance : IGame
{
    public bool isInitialized = false;

    public UICourtshipDance uiCourtship;
    public CourtshipDanceData gameData;

    private CommandGenerator commandGenerator;
    private Dictionary<int, Queue<Queue<BubbleInfo>>> teamPoolDic;
    public List<PlayerInfo> players = new();   
    private TaskCompletionSource<bool> sourceTcs;

    public bool isTeamGame = false;
    public bool isBoardReady = false;
    public bool isGameOver = false;
    public Dictionary<int, List<PlayerInfo>> teamDic;

    public GameCourtshipDance()
    {
    }

    public async void Init(params object[] param)
    {
        await Initialize(param);
    }

    public async Task Initialize(params object[] param)
    {
        try
        {
            sourceTcs = new();
            gameData = new CourtshipDanceData();
            gameData.Init();

            if (param[0] is S2C_DanceMiniGameReadyNotification response)
            {
                foreach (var p in response.Players)
                {
                    this.players.Add(p);
                }
            }
            else if (param[0] is RepeatedField<PlayerInfo> players)
            {
                foreach (var p in players)
                {
                    this.players.Add(p);
                }
            }

            teamDic = new();
            foreach (var p in players)
            {
                if (!teamDic.ContainsKey(p.TeamNumber))
                {
                    List<PlayerInfo> list = new()
                {
                    p
                };
                    teamDic.Add(p.TeamNumber, list);
                }
                else
                {
                    teamDic[p.TeamNumber].Add(p);
                }
            }

            if (teamDic[1].Count >= 2)
            {
                isTeamGame = true;
            }

            uiCourtship = await UIManager.Show<UICourtshipDance>(gameData);
            MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameCourtshipDance>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
            await MinigameManager.Instance.MakeMapDance();
            // 토큰 배치 및 세팅하기
            ResetPlayers(players);

            if (GameManager.Instance.myInfo.SessionId == players[0].SessionId)
            {
                commandGenerator = new CommandGenerator();
                commandGenerator.InitTeamGame(teamDic);
                teamPoolDic = commandGenerator.GetTeamPoolDic();
            }

            // 커맨드보드 제작과 전송완료대기 리퀘스트 패킷, 그 응답,알림 패킷
            /* 405 */
            if (commandGenerator != null)
            {
                RepeatedField<DancePool> sp = CommandGenerator.ConvertToDancePools(teamPoolDic, teamDic);
                GamePacket packet = new();
                packet.DanceTableCreateRequest = new()
                {
                    SessionId = GameManager.Instance.myInfo.SessionId,
                };
                packet.DanceTableCreateRequest.DancePools.Add(sp);
                SocketManager.Instance.OnSend(packet);
            }

            bool isSuccess = await Task.WhenAny(sourceTcs.Task, Task.Delay(60000)) == sourceTcs.Task ? await sourceTcs.Task : false;
            if (!isSuccess)
            {
                UnityEngine.Debug.LogAssertion("Timeout : DanceTableNotification ");
                // TODO:: 어플 Quit?
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                UnityEngine.Application.Quit();
#endif
            }
            isBoardReady = isSuccess;

            uiCourtship.MakeCommandBoard(teamDic, teamPoolDic);
            SoundManager.Instance.PlayBGM(BGMType.Dance);
            isInitialized = true;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error in Init: {ex.Message}");
        }
    }

    public void SetTeamPoolDic(RepeatedField<DancePool> dancePools)
    {
        try
        {
            teamPoolDic = CommandGenerator.ConvertToTeamPoolDic(dancePools);
            if (teamPoolDic != null)
            {
                sourceTcs.TrySetResult(true);
            }
            else
            {
                UnityEngine.Debug.LogAssertion("teamPoolDic is null !!");
                sourceTcs.TrySetResult(false);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error in SetTeamPoolDic: {ex.Message}");
            sourceTcs.TrySetException(ex);
        }
    }

    public async void GameStart(params object[] param)
    {
        if (param[0] is long startTime)
        {
            Action action = () =>
            {
                MinigameManager.Instance.GetMyToken().EnableInputSystem(eGameType.GameCourtshipDance);
                uiCourtship.ShowTimer();
            };
            uiCourtship.StartTimer();
            var map = await MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
            map.ShowIndicator();
            await UIManager.Show<UICountdown>(startTime, 3, action);
        }
    }

    public async void BeforeGameEnd()
    {
        var map = await MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
        foreach (var player in players)
        {
            var token = MinigameManager.Instance.GetMiniToken(player.SessionId);
            map.TokenReset(token);
        }
    }

    public async void GameEnd(List<(int Rank, string SessionId)> ranks, long boardTime)
    {
        foreach (var mini in MinigameManager.Instance.miniTokens)
        {
            mini.gameObject.SetActive(false);
        }
        UIManager.Hide<UICourtshipDance>();
        await UIManager.Show<UIMinigameResult>(ranks, boardTime + 1500);
    }

    #region 초기화

    private async void ResetPlayers(List<PlayerInfo> players)
    {
        var map = await MinigameManager.Instance.GetMap<MapGameCourtshipDance>();
        int num = 0;
        int[] teamSpawnCount = new int[2] { 0, 0 };
        foreach (var p in players)
        {//미니 토큰 위치 초기화
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            map.TokenInit(miniToken);
            
            if (!isTeamGame)
            {
                //개인전 세팅. 
                miniToken.transform.position = map.spawnPosition[num].position;
                miniToken.transform.rotation = map.spawnPosition[num].rotation;                
                miniToken.MiniData.rotY = map.spawnPosition[num].rotation.eulerAngles.y;
            }
            else
            {
                int teamIndex = (p.TeamNumber % 2 == 1) ? 0 : 1;
                int spawnIndex = teamIndex + (teamSpawnCount[teamIndex] * 2);

                miniToken.transform.position = map.spawnPosition[spawnIndex].position;
                miniToken.MiniData.rotY = map.spawnPosition[spawnIndex].rotation.eulerAngles.y;
                teamSpawnCount[teamIndex]++;
            }
            num++;
        }
    }
    #endregion
       


    public int GetMyTeam()
    {
        foreach (var p in players)
        {
            if (p.SessionId == GameManager.Instance.myInfo.SessionId)
            {
                return p.TeamNumber;
            }
        }
        return -1;
    }
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

    public void RemovePlayers(string disconnectedSessionId)
    {
        foreach (var p in players)
        {
            if(p.SessionId == disconnectedSessionId)
            {
                players.Remove(p);
                break;
            }
        }
    }

    public void DisableUI()
    {
        UIManager.Hide<UICourtshipDance>();
    }
}