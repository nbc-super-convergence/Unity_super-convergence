using System.Collections.Generic;
using UnityEngine;

public class UICommandBoardHandler : UIBase
{
    //private List<CommandBoard> CommandBoards = new();
    public List<Transform> spawnPosition;

    public Dictionary<string, CommandBoard> boardDic = new();

    public async void MakeCommandBoard(List<Player> players)
    {
        for (int i = 0; i < players.Count; ++i)
        {
            // 프리팹 생성.
            var board = Instantiate(await ResourceManager.Instance.LoadAsset<CommandBoard>("CommandBoard", eAddressableType.Prefab), spawnPosition[i]);
            board.transform.localPosition = Vector3.zero;
            //board.SetPool(MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().GetCommandInfoPool());
            if(MinigameManager.Instance.GetMiniGame<GameCourtshipDance>().commandPoolDic.TryGetValue(players[i].SessionId, out Queue<Queue<BubbleInfo>> pool))
            {
                board.SetSessionId(players[i].SessionId);
                board.SetTeamId(players[i].TeamId);
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

}