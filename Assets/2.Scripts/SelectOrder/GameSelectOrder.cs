using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectOrder : IGame
{
    //다트판
    public SelectOrderPannel DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

    //UI
    [SerializeField] private RectTransform targetUI;    //타겟 지점
    [SerializeField] private SelectOrderDartUI dartPowerUI;
    [SerializeField] private Transform resultGroup; //다트 결과
    private List<SelectOrderResultUI> resultsUI = new();

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
    private void SettingBasic()
    {
        for (int i = 0; i < resultGroup.childCount; i++)
        {
            resultsUI.Add(resultGroup.GetChild(i).GetComponent<SelectOrderResultUI>());
        }

        minAim = -20f;
        maxAim = 20f;
        minForce = 1.5f;
        maxForce = 3f;

        //dartUI.SetAimLimit(minAim, maxAim);
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
    /// 지금부터 시작
    /// </summary>
    public void BeginSelectOrder()
    {
        DartOrder[nowPlayer].gameObject.SetActive(true);
        resultsUI[nowPlayer].SetMyTurn();
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
        resultsUI[nowPlayer].SetFinish();

        nowPlayer++;
        if (nowPlayer < DartOrder.Count)    //최대 인원보다 초과되지 않게
        {
            resultsUI[nowPlayer].SetMyTurn();
            DartOrder[nowPlayer].gameObject.SetActive(true);
        }
        else
        {
            DartPannel.isMove = false;  //판은 멈춰라
            DartPannel.DistanceRank();
        }
    }

    /// <summary>
    /// 각 플레이어의 순서와 점수를 부여
    /// </summary>
    public void FinishSelectOrder()
    {
        for (int i = 0; i < DartOrder.Count; i++)
        {
            resultsUI[i].SetRank(DartOrder[i].MyRank);
            resultsUI[i].SetScore(DartOrder[i].MyDistance);
        }
    }

    #region IGame
    public async void Init(params object[] param)
    {
        MinigameManager.Instance.curMap = await ResourceManager.Instance.LoadAsset<MapGameSelectOrder>($"Map{MinigameManager.gameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap<MapGameSelectOrder>();
        SettingBasic();
    }

    public void GameStart(params object[] param)
    {
        BeginSelectOrder();
    }
    #endregion
}
