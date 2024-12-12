using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICourtshipDance : UIBase
{
    private CourtshipDanceData gameData;
    private GameCourtshipDance game;
    private bool isTeamGame;
    
    [SerializeField] private GameObject end;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] public AudioClip[] sfxClips;

    public List<Transform> spawnPosition;
    public Dictionary<string, CommandBoard> boardDic = new();

    public override void Opened(object[] param)
    {
        gameData = param[0] as CourtshipDanceData;
        game = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>();
        isTeamGame = game.isTeamGame;
    }

    public async void MakeCommandBoard(List<PlayerInfo> players)
    {
        for (int i = 0; i < players.Count; ++i)
        {
            // 프리팹 생성.
            var board = Instantiate(await ResourceManager.Instance.LoadAsset<CommandBoard>("CommandBoard", eAddressableType.Prefab), spawnPosition[i]);
            board.transform.localPosition = Vector3.zero;
            if(game.commandPoolDic.TryGetValue(players[i].SessionId, out Queue<Queue<BubbleInfo>> pool))
            {
                board.SetSessionId(players[i].SessionId);
                board.SetTeamId(players[i].TeamNumber);
                board.SetPool(pool);
                board.Init();
            }
            boardDic.Add(players[i].SessionId, board);
        }
    }

    public async void MakeCommandBoard(Dictionary<int, List<PlayerInfo>> teamDic, Dictionary<int, Queue<Queue<BubbleInfo>>> teamPoolDic)
    {
        for (int i = 0; i < teamDic.Count; ++i)
        {
            int num = i + 1;
            var board = Instantiate(await ResourceManager.Instance.LoadAsset<CommandBoard>("CommandBoard", eAddressableType.Prefab), spawnPosition[i]);
            board.transform.localPosition = Vector3.zero;
            if (teamPoolDic.TryGetValue(num, out Queue<Queue<BubbleInfo>> pool))
            {
                board.SetSessionId(teamDic[num][0].SessionId);
                board.SetTeamId(num);
                board.SetPool(pool);
                board.Init();
            }
            //boardDic.Add(players[i].SessionId, board);
        }
    }

    public void Next(string sessionId)
    {
        boardDic[sessionId].MakeNextBoard();
    }
    
    public void ShowDanceBoard()
    {
        foreach( var item in boardDic.Values)
        {
            item.MakeNextBoard();
        }
    }

    // 카운트다운이 끝나면 실행하기
    public void StartTimer()
    {
        timeText.gameObject.SetActive(true);
        StartCoroutine(UIUtils.DecreaseTimeCoroutine(gameData.totalTime, timeText));
    }


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

        /*필요 데이터 파싱*/   // 세션Id, 등수
        Dictionary<string, int> rankings = new();
        for (int i = 0; i < teamResults.Count; ++i)
        {
            int rank = i + 1;
            rankings.Add(teamResults[i].SessionId[0], rank);
        }

        StartCoroutine(GameOverText(teamResults, rankings, response.EndTime));
    }

    public IEnumerator GameOverText(List<TeamResult> teamResults, Dictionary<string, int> rankings, long endTime)
    {
        end.SetActive(true);
        yield return new WaitForSeconds(2f);
        end.SetActive(false);
        MinigameManager.Instance.GetMiniToken(teamResults[0].SessionId[0]).MiniData.CurState = State.DanceUp;
        // 1등은 우승 애니메이션 재생 루프
        var info = MinigameManager.Instance.GetMiniToken(teamResults[0].SessionId[0]).GetAnimator().GetCurrentAnimatorStateInfo(1);

        yield return new WaitForSeconds(2f);

        game.BeforeGameEnd();
        Destroy(MinigameManager.Instance.curMap.gameObject);
        MinigameManager.Instance.boardCamera.SetActive(true);

        game.GameEnd(rankings, endTime);
    }
    #endregion
}
