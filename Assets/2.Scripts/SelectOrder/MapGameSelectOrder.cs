using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGameSelectOrder : MapBase
{
    [SerializeField] private SelectOrderPannel DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

    #region UI 속성
    [SerializeField] private SelectOrderDartUI dartPowerUI;
    [SerializeField] private Transform resultGroup; //다트 결과
    private List<SelectOrderResultUI> resultsUI = new();
    #endregion

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

    private void Awake()
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

    private void Start()
    {
        BeginSelectOrder();
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
    private void BeginSelectOrder()
    {
        DartOrder[nowPlayer].gameObject.SetActive(true);
        resultsUI[nowPlayer].SetMyTurn();
    }
}