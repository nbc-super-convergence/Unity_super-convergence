using System.Collections.Generic;

//땅을 제일 많이 사지 않은 사람
public class LowPurchaseCount : IGameResult
{
    public List<int> Result()
    {
        List<int> result = new List<int>();
        List<KeyValuePair<int, int>> list = new();
        List<BoardTokenHandler> players = BoardManager.Instance.playerTokenHandlers;

        for (int i = 0; i < players.Count; i++)
        {
            int value = players[i].data.gameData.purchaseArea; //구매수

            list.Add(new KeyValuePair<int, int>(value, i));
        }

        list.Sort((a, b) => { return a.Key.CompareTo(b.Key); }); //오름차순 정렬

        int key = list[0].Key;

        for (int i = 0; i < list.Count; i++)
            if (key == list[i].Key) result.Add(list[i].Value);

        return result;
    }
}
