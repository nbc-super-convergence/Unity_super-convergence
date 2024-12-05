﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOrderManager : Singleton<SelectOrderManager>
{
    //다트판
    public GameObject DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

    //조절 UI
    [SerializeField] private SelectOrderDartUI dartUI;
    [SerializeField] private SelectOrderResultUI resultUI;

    private int yourPlayer = 1; //(임시) 현재 플레이어 (서버에서 받을거라)

    //조절 속성
    public float minAim { get; private set; }
    public float maxAim { get; private set; }

    //힘 속성
    public float minForce { get; private set; }
    public float maxForce { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        minAim = 0f;
        maxAim = 20f;
        minForce = 1.5f;
        maxForce = 3f;

        dartUI.SetAimLimit(minAim, maxAim);
        dartUI.SetForceLimit(minForce, maxForce);
    }

    private void Start()
    {
        HideResultUI();
    }

    private void Update()
    {
        //내 다트를 받으면 해당 다트의 속성들을 UI에 적용
        dartUI.GetAim(DartOrder[yourPlayer - 1].CurAim);
        dartUI.GetForce(DartOrder[yourPlayer - 1].CurForce);
    }

    /// <summary>
    /// 던졌으면 UI 감추기
    /// </summary>
    public void HideDartUI()
    {
        dartUI.gameObject.SetActive(false);
    }

    public void ShowResultUI()
    {
        resultUI.gameObject.SetActive(true);
    }
    private void HideResultUI()
    {
        resultUI.gameObject.SetActive(false);
    }
}
