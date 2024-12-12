//using System.Collections.Generic;

//public class LoseCount : IGameResult
//{
//    public List<int> Result()
//    {
//        List<int> result = new List<int>();
//        List<KeyValuePair<int, int>> list = new();
//        List<BoardTokenHandler> players = BoardManager.Instance.playerTokenHandlers;

//        for (int i = 0; i < players.Count; i++)
//        {
//            int value = players[i].data.gameData.loseMiniGame; //미니게임 패배수

//            list.Add(new KeyValuePair<int, int>(value, i));
//        }

//        list.Sort((a, b) => { return b.Key.CompareTo(a.Key); }); //내림차순

//        int key = list[0].Key;

//        for (int i = 0; i < list.Count; i++)
//            if (key == list[i].Key) result.Add(list[i].Value); 

//        return result;
//    }
//}
