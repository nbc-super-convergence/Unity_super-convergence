//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class TestBonus : MonoBehaviour
//{
//    private List<IGameResult> bonus;
//    public List<TMP_Text> tMP_Texts;

//    private void Awake()
//    {
//        SetBonus();

//        foreach (var g in bonus)
//        {
//            List<int> list = g.Result();
//            Debug.Log(g.GetType());

//            var players = BoardManager.Instance.playerTokenHandlers;
//            foreach (var i in list)
//                players[i].data.trophyAmount += 1;
//        }
//    }

//    private void Update()
//    {
//        for(int i = 0; i < tMP_Texts.Count; i++)
//        {
//            tMP_Texts[i].text = BoardManager.Instance.playerTokenHandlers[i].data.trophyAmount.ToString();
//        }
//    }

//    private void SetBonus()
//    {
//        bonus = new();
//        List<int> num = new();

//        for (int i = 0; i < 3;)
//        {
//            int rand = UnityEngine.Random.Range(0, 13);

//            if (num.Contains(rand)) continue;
//            num.Add(rand);
//            //***주의 열지마시오, 진짜 경고했음
//            switch (rand)
//            {
//                case 0:
//                    bonus.Add(new FastCoinZero());
//                    break;
//                case 1:
//                    bonus.Add(new HighComebackCount());
//                    break;
//                case 2:
//                    bonus.Add(new HighDiceCount());
//                    break;
//                case 3:
//                    bonus.Add(new HighPaymentCount());
//                    break;
//                case 4:
//                    bonus.Add(new HighPurchaseCount());
//                    break;
//                case 5:
//                    bonus.Add(new HighSaveCoin());
//                    break;
//                case 6:
//                    bonus.Add(new HighSellCount());
//                    break;
//                case 7:
//                    bonus.Add(new HighTaxCount());
//                    break;
//                case 8:
//                    bonus.Add(new LoseCount());
//                    break;
//                case 9:
//                    bonus.Add(new LowDiceCount());
//                    break;
//                case 10:
//                    bonus.Add(new LowPurchaseCount());
//                    break;
//                case 11:
//                    bonus.Add(new NoneTrophy());
//                    break;
//                case 12:
//                    bonus.Add(new WinCount());
//                    break;
//            }
//            i++;
//        }
//    }
//}
