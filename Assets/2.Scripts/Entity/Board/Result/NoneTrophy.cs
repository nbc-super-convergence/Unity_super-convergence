using System.Collections.Generic;

//트로피가 없는사람
public class NoneTrophy : IGameResult
{
    public List<int> Result()
    {
        List<int> result = new List<int>();
        List<BoardTokenHandler> players = BoardManager.Instance.playerTokenHandlers;

        for(int i = 0; i < players.Count; i++)
        {
            int value = players[i].data.trophyAmount;

            if(value == 0) result.Add(i);
        }

        return result;
    }
}
