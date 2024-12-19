using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICourtshipDance : UIBase
{
    [Header("Game Objects")]
    [SerializeField] private GameObject end;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] public AudioClip[] sfxClips;
    public List<Transform> spawnPosition;

    // -- Cached References -- //
    private CourtshipDanceData gameData;
    private GameCourtshipDance game;
    public Dictionary<int, CommandBoard> boardDic = new();
    [HideInInspector] public CommandBoard myBoard;

    // -- State Flags -- //
    private bool isTeamGame;

    public override void Opened(object[] param)
    {
        gameData = param[0] as CourtshipDanceData;
        game = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>();
        isTeamGame = game.isTeamGame;
    }

    #region 커맨드보드 UI 초기화
    public async void MakeCommandBoard(Dictionary<int, List<PlayerInfo>> teamDic, Dictionary<int, Queue<Queue<BubbleInfo>>> teamPoolDic)
    {
        int num = 0;
        foreach( var team in teamDic)
        {
            var board = Instantiate(await ResourceManager.Instance.LoadAsset<CommandBoard>("CommandBoard", eAddressableType.Prefab), spawnPosition[num++]);
            board.transform.localPosition = Vector3.zero;
            if (teamPoolDic.TryGetValue(team.Key, out Queue<Queue<BubbleInfo>> pool))
            {
                foreach (var id in team.Value)
                {
                    board.teamSessionIds.Add(id.SessionId);
                }
                board.Init(team.Key, pool);
            }
            boardDic.Add(team.Key, board);

            board.MakeNextBoard();

            if(game.GetMyTeam() == team.Key)
            {
                myBoard = board;
            }
        }
    }
    #endregion

    #region Timer
    public void StartTimer()
    {
        StartCoroutine(UIUtils.DecreaseTimeCoroutine(gameData.timerStartValue, timeText));
    }

    public void ShowTimer()
    {
        timeText.gameObject.SetActive(true);
    }
    #endregion

    #region 게임오버
    public void GameOver(S2C_DanceGameOverNotification response)
    {        
        List<int> teamRank = new();
        foreach (int i in response.TeamRank)
        {
            teamRank.Add(i);
        }

        List<TeamResult> teamResults = new();
        foreach (var teamResult in response.Result)
        {
            teamResults.Add(teamResult);
        }

        teamResults.Sort((a, b) =>
        {
            if (a.Score != b.Score)
                return b.Score.CompareTo(a.Score);
            else
                return a.EndTime.CompareTo(b.EndTime);
        });

        List<(int Rank, string SessionId)> rankings = new();
        int rank = 1;
        foreach (var teamResult in teamResults)
        {
            for (int i = 0; i < teamResult.SessionId.Count; ++i)
            {
                rankings.Add((rank, teamResult.SessionId[i]));
            }
            rank++;
        }
        timeText.gameObject.SetActive(false);
        StartCoroutine(GameOverText(teamResults, rankings, response.EndTime + 7000));
    }

    public IEnumerator GameOverText(List<TeamResult> teamResults, List<(int Rank, string SessionId)> rankings, long endTime)
    {
        end.SetActive(true);
        yield return new WaitForSeconds(2f);
        end.SetActive(false);
        List<MiniToken> winTokens = new();
        RepeatedField<string> winSessionIds = teamResults[0].SessionId;
        foreach (var sessionId in winSessionIds)
        {
            winTokens.Add(MinigameManager.Instance.GetMiniToken(sessionId));
        }
        foreach (var winToken in winTokens)
        {
            winToken.MiniData.CurState = State.DanceUp;
            winToken.GetAnimator().GetCurrentAnimatorStateInfo(1);
        }

        yield return new WaitForSeconds(2f);

        game.BeforeGameEnd();
        Destroy(MinigameManager.Instance.curMap.gameObject);
        MinigameManager.Instance.boardCamera.SetActive(true);

        game.GameEnd(rankings, endTime);
    }
    #endregion

    #region 게임 중 Disconnect
    public void DisconnectNoti(string disconnectedSessionId, string replacementSessionId)
    {
        if (!isTeamGame) return;

        if(myBoard.teamSessionIds.Contains(disconnectedSessionId))
        {
            // 우리 팀 처리
            myBoard.ChangeInfoPool(disconnectedSessionId, replacementSessionId);
            myBoard.teamSessionIds.Remove(disconnectedSessionId);
        }
        else
        {
            // 남의 팀 처리
            int disconnectTeam = game.GetPlayerTeam(disconnectedSessionId);
            boardDic[disconnectTeam].ChangeInfoPool(disconnectedSessionId, replacementSessionId);
            boardDic[disconnectTeam].teamSessionIds.Remove(disconnectedSessionId);
        }
        game.RemovePlayers(disconnectedSessionId);
    }
    #endregion
}