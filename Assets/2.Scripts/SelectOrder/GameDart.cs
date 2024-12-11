using System.Collections.Generic;
using UnityEngine;

public class GameDart : IGame
{
    private UIMinigameDart ingameUI;

    //다트판
    public SelectOrderPanel DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

    //UI
    [SerializeField] private RectTransform targetUI;    //타겟 지점
    [SerializeField] private SelectOrderDartUI dartPowerUI;
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

    #region 조절 속성
    //조절 속성
    public float minAim { get; private set; }
    public float maxAim { get; private set; }

    //힘 속성
    public float minForce { get; private set; }
    public float maxForce { get; private set; }
    #endregion

    /// <summary>
    ///  기본 설정 세팅
    /// </summary>
    private void SettingData()
    {
        minAim = -20f;
        maxAim = 20f;
        minForce = 1.5f;
        maxForce = 3f;

        dartPowerUI.SetForceLimit(minForce, maxForce);
        for (int i = 0; i < DartOrder.Count; i++)
        {
            DartOrder[i].SetAimRange(minAim, maxAim);
            DartOrder[i].SetForceRange(minForce, maxForce);
        }
    }

    private void Update()
    {
        if (nowPlayer < DartOrder.Count)
        {
            //내 다트를 받으면 해당 다트의 속성들을 UI에 적용
            dartPowerUI.GetForce(DartOrder[nowPlayer].CurForce);
        }
    }

    /// <summary>
    /// 던졌으면 UI 감추기
    /// </summary>
    public void HideDartUI()
    {
        //dartPowerUI.HideDirect();
        dartPowerUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 다음 차례
    /// </summary>
    public void NextDart()
    {

        nowPlayer++;
        if (nowPlayer < DartOrder.Count)    //최대 인원보다 초과되지 않게
        {
            DartOrder[nowPlayer].gameObject.SetActive(true);
        }
        else
        {
            DartPannel.isMove = false;  //판은 멈춰라
            DistanceRank();
        }
    }

    private List<float> distanceRank = new List<float>();    //다트 거리의 매겨줄 랭킹

    //중심과 가까운 다트가 우선순위
    public void DistanceRank()
    {
        int rank = 1;

        List<SelectOrderDart> dartOrder = MinigameManager.Instance.GetMiniGame<GameDart>().DartOrder;


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
    }

    public async void GameStart(params object[] param)
    {
        ingameUI = await UIManager.Show<UIMinigameDart>();
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }
    #endregion
}
