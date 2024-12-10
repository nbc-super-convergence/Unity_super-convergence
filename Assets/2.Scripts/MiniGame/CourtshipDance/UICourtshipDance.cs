using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICourtshipDance : UIBase
{
    [SerializeField] private GameObject end;
    [SerializeField] private TextMeshProUGUI timeText;

    public List<Transform> spawnPosition;
    public Dictionary<string, CommandBoard> boardDic = new();

    private CourtshipDanceData gameData;
    GameCourtshipDance game;
    
    public override void Opened(object[] param)
    {
        gameData = param[0] as CourtshipDanceData;
        game = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>();
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

    public void Next(string sessionId)
    {
        boardDic[sessionId].MakeNextBoard();
    }

    // 카운트다운이 끝나면 실행하기
    public void StartTime()
    {
        StartCoroutine(UIUtils.DecreaseTimeCoroutine(gameData.totalTime, timeText));
    }



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

        StartCoroutine(GameOverText(teamResults, rankings));
    }
    public IEnumerator GameOverText(List<TeamResult> teamResults, Dictionary<string, int> rankings)
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

        //MinigameManager.Instance.curMiniGame.GameEnd(rankings, response.EndTime);
        UIManager.Hide<UICourtshipDance>();
    }
}