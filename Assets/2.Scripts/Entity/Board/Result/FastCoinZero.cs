using System.Collections.Generic;
public class FastCoinZero : IGameResult
{
    List<int> result;

    public FastCoinZero()
    {
        result = new();
        BoardManager.Instance.OnEvent += CoinCheck;
    }

    private void CoinCheck()
    {
        var list = BoardManager.Instance.playerTokenHandlers;

        for(int i = 0; i < list.Count; i++)
        {
            var data = list[i].data;

            if (data.keyAmount == 0) result.Add(i);
        }

        if(result.Count > 0)
            BoardManager.Instance.OnEvent -= CoinCheck;
    }

    public List<int> Result()
    {
        return result;
    }
}

