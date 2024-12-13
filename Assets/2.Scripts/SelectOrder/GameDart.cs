using System.Collections.Generic;
using UnityEngine;

public class GameDart : IGame
{
    private UIMinigameDart ingameUI;

    //다트판
    public GameDartPanel DartPannel;

    //다트그룹
    public List<DartPlayer> DartOrder;

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

    /// <summary>
    ///  기본 설정 세팅
    /// </summary>
    private void SettingData()
    {
    }

    private void Update()
    {
        if (nowPlayer < DartOrder.Count)
        {
            //내 다트를 받으면 해당 다트의 속성들을 UI에 적용
        }
    }

    /// <summary>
    /// 던졌으면 UI 감추기
    /// </summary>
    public void HideDartUI()
    {

    }

    /// <summary>
    /// 다음 차례
    /// </summary>
    public void NextDart()
    {

        nowPlayer++;
        if (nowPlayer < DartOrder.Count)    //최대 인원보다 초과되지 않게
        {
            Debug.Log("다음 사람");
            DartOrder[nowPlayer].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("결과");
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

        MinigameManager.Instance.GetMiniGame<GameDart>().FinishSelectOrder();
    }

    /// <summary>
    /// 각 플레이어의 순서와 점수를 부여
    /// </summary>
    public void FinishSelectOrder()
    {

    }

    #region IGame
    public async void Init(params object[] param)
    {
        MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameDart>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap<MapGameDart>();
        SettingData();

        //DartOrder데이터 설정
        DartOrder = MinigameManager.Instance.GetMap<MapGameDart>().DartOrder;
        if (param.Length > 0 && param[0] is S2C_DiceGameNotification response)
        {

        }
    }

    public async void GameStart(params object[] param)
    {
        ingameUI = await UIManager.Show<UIMinigameDart>();
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }
    public void DisableUI()
    {

    }
    #endregion
}
