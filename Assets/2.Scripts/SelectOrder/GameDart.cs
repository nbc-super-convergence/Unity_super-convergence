using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDart : IGame
{
    private UIMinigameDart ingameUI;

    //다트판
    private GameDartPanel DartPannel;

    //다트그룹
    private List<DartPlayer> DartOrder;

    //UI
    [SerializeField] private RectTransform targetUI;    //타겟 지점
    [SerializeField] private Transform resultGroup; //다트 결과

    [SerializeField] private Transform Title;   //타이틀 및 설명

    private int nowPlayer = 0;  // 현재 플레이어 차례
    private int missRank = 4;
    public int MissRank
    {
        get
        {
            return missRank--;
        }
    }   //빗나간 랭크

    private int curRound = 1;   //현재 라운드
    private int maxRound = 9;   //최대 라운드

    private int playerCount;    //현재 플레이어 참여 인원

    /// <summary>
    /// 다음 차례
    /// </summary>
    public void NextDart()
    {
        UIManager.Get<UIMinigameDart>().SetFinish(nowPlayer);
        nowPlayer++;

        if (nowPlayer < playerCount)    //최대 인원보다 초과되지 않게
        {
            //Debug.Log("다음 사람");
            DartPannel.SetClient(nowPlayer);
            DartOrder[nowPlayer].gameObject.SetActive(true);
            UIManager.Get<UIMinigameDart>().SetMyTurn(nowPlayer);
        }
        else
        {
            NextRound();

            //DistanceRank();
        }
    }

    /// <summary>
    /// 다음 라운드
    /// </summary>
    private void NextRound()
    {
        curRound++;
        UIManager.Get<UIMinigameDart>().SetRound(curRound);

        //다트 초기
        for (int i = 0; i < playerCount; i++)
        {
            UIManager.Get<UIMinigameDart>().SetReady(i);
            DartOrder[i].ResetDart();
        }
        nowPlayer = 0;
        DartOrder[nowPlayer].gameObject.SetActive(true);

        if(curRound > maxRound)
        {
            DartPannel.isMove = false;  //판은 멈춰라

            //결과
            //GameOverNotification

        }
    }

    private List<float> distanceRank = new List<float>();    //다트 거리의 매겨줄 랭킹

    //중심과 가까운 다트가 우선순위
    public void DistanceRank()
    {   
        int rank = 1;

        List<DartPlayer> dartOrder = MinigameManager.Instance.GetMiniGame<GameDart>().DartOrder;


        foreach (var dart in dartOrder)
            distanceRank.Add(dart.MyDistance);

        distanceRank.Sort();

        //정렬후 랭킹
        for (int i = 0; i < distanceRank.Count; i++)
        {
            foreach (var dart in dartOrder)
            {
                if (dart.MyDistance.Equals(distanceRank[i]))
                {
                    if (dart.MyDistance >= 10f)
                        continue;
                    else
                    {
                        dart.MyRank = rank;
                        rank++;
                    }
                }
            }
        }
    }

    private async void SettingDart(RepeatedField<S2C_DartMiniGameReadyNotification.Types.startPlayers> players)
    {
        playerCount = players.Count;
        var map = await MinigameManager.Instance.GetMap<MapGameDart>();
        map.SetDartPlayers(playerCount);
        foreach(var p in players)
        {
            string nickname = GameManager.Instance.SessionDic[p.SessionId].Nickname;
        }
    }

    #region IGame
    public async void Init(params object[] param)
    {
        MinigameManager.Instance.curMap =
            await ResourceManager.Instance.LoadAsset<MapGameDart>
            ($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap<MapGameDart>();

        //DartOrder데이터 설정
        var map = await MinigameManager.Instance.GetMap<MapGameDart>();
        DartOrder = map.DartOrder;
        DartPannel = map.DartPanel;

        if (param.Length > 0 && param[0] is S2C_DartMiniGameReadyNotification response)
        {
            SettingDart(response.Players);
        }
        else
        {
            Debug.LogError("param parsing error : startPlayers");
        }
    }

    public async void GameStart(params object[] param)
    {
        ingameUI = await UIManager.Show<UIMinigameDart>();
        MinigameManager.Instance.GetMyToken().EnableInputSystem();

        var map = await MinigameManager.Instance.GetMap<MapGameDart>();
        map.BeginSelectOrder();
        map.DartPanel.SetClient(0);
    }
    public void DisableUI()
    {
        UIManager.Hide<UIMinigameDart>();
    }
    #endregion
}