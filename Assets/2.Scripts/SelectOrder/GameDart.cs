using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDart : IGame
{
    private GameDartData gameData;
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

    private int curRound = 0;   //현재 라운드
    private int maxRound = 3;   //최대 라운드

    private int playerCount;    //현재 플레이어 참여 인원

    /// <summary>
    /// 다음 차례
    /// </summary>
    public void NextDart()
    {

        nowPlayer++;
        if (nowPlayer < playerCount)    //최대 인원보다 초과되지 않게
        {
            //Debug.Log("다음 사람");
            DartOrder[nowPlayer].gameObject.SetActive(true);
        }
        else
        {
            //Debug.Log("결과");
            DartPannel.isMove = false;  //판은 멈춰라
            DistanceRank();
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

    public void PannelMoveEvent()
    {
        Debug.Log("움직인다");
        DartPannel.MoveEvent();
    }

    private void SettingDart(RepeatedField<S2C_DartMiniGameReadyNotification.Types.startPlayers> players)
    {
        playerCount = players.Count;
        MinigameManager.Instance.GetMap<MapGameDart>().SetDartPlayers(playerCount);

        foreach(var p in players)
        {

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
        DartOrder = MinigameManager.Instance.GetMap<MapGameDart>().DartOrder;
        DartPannel = MinigameManager.Instance.GetMap<MapGameDart>().DartPanel;

        if (param.Length > 0 && param[0] is S2C_DartMiniGameReadyNotification response)
        {
            SettingDart(response.Players);
            MinigameManager.Instance.GetMap<MapGameDart>().BeginSelectOrder();
        }
        else
        {
            Debug.LogError("param parsing error : startPlayers");
        }
    }

    public async void GameStart(params object[] param)
    {
        ingameUI = await UIManager.Show<UIMinigameDart>(gameData);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }
    public void DisableUI()
    {
        UIManager.Hide<UIMinigameDart>();
    }
    #endregion
}

public class GameDartData : IGameData
{
    public void Init()
    {

    }
}