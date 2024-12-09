﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOrderManager : Singleton<SelectOrderManager>
{
    //다트판
    public GameObject DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

    //UI
    [SerializeField] private RectTransform targetUI;    //타겟 지점
    [SerializeField] private SelectOrderDartUI dartUI;
    [SerializeField] private Transform resultGroup; //다트 결과
    private List<SelectOrderResultUI> resultsUI = new();

    [SerializeField] private Transform Title;   //타이틀 및 설명

    private int nowPlayer = 0;  // 현재 플레이어 차례

    private bool nowStart = false;  //지금부터 시작

    #region 조절 속성
    //조절 속성
    public float minAim { get; private set; }
    public float maxAim { get; private set; }

    //힘 속성
    public float minForce { get; private set; }
    public float maxForce { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < resultGroup.childCount; i++)
        {
            resultsUI.Add(resultGroup.GetChild(i).GetComponent<SelectOrderResultUI>());
        }

        minAim = -20f;
        maxAim = 20f;
        minForce = 1.5f;
        maxForce = 3f;

        //dartUI.SetAimLimit(minAim, maxAim);
        dartUI.SetForceLimit(minForce, maxForce);
    }

    private void Update()
    {
        //스페이스바 누르면 시작 (임시로)
        if(Input.GetKeyDown(KeyCode.Space))
        {
            nowStart = true;
            Title.gameObject.SetActive(false);
            BeginSelectOrder();
        }

        if (nowPlayer < DartOrder.Count)
        {
            //내 다트를 받으면 해당 다트의 속성들을 UI에 적용
            dartUI.GetForce(DartOrder[nowPlayer].CurForce);
            SetTargetDart();
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

    /// <summary>
    /// 던졌으면 UI 감추기
    /// </summary>
    public void HideDartUI()
    {
        dartUI.gameObject.SetActive(false);
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

    /// <summary>
    /// 표적을 UI표시
    /// </summary>
    private void SetTargetDart()
    {
        Vector3 set = DartOrder[nowPlayer].TargetPosition();
        targetUI.localPosition = Camera.main.ScreenToWorldPoint(set);
    }
}
