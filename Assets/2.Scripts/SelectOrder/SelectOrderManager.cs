using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOrderManager : Singleton<SelectOrderManager>
{
    //다트판
    public GameObject DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

    //UI
    [SerializeField] private SelectOrderDartUI dartUI;
    [SerializeField] private Transform resultGroup;
    private List<SelectOrderResultUI> resultsUI = new();

    private int nowPlayer = 0;  // 현재 플레이어 차례

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

        minAim = 0f;
        maxAim = 20f;
        minForce = 1.5f;
        maxForce = 3f;

        dartUI.SetAimLimit(minAim, maxAim);
        dartUI.SetForceLimit(minForce, maxForce);
    }

    private void Start()
    {
        DartOrder[nowPlayer].gameObject.SetActive(true);
        resultsUI[nowPlayer].SetMyTurn();
    }

    private void Update()
    {
        if (nowPlayer < DartOrder.Count)
        {
            //내 다트를 받으면 해당 다트의 속성들을 UI에 적용
            dartUI.GetAim(DartOrder[nowPlayer].CurAim);
            dartUI.GetForce(DartOrder[nowPlayer].CurForce);
        }
    }

    /// <summary>
    /// 던졌으면 UI 감추기
    /// </summary>
    public void HideDartUI()
    {
        dartUI.gameObject.SetActive(false);
    }

    public void NextDart()
    {
        resultsUI[nowPlayer].SetFinish(DartOrder[nowPlayer].MyDistance);

        nowPlayer++;
        if (nowPlayer < DartOrder.Count)    //최대 인원보다 초과되지 않게
        {
            resultsUI[nowPlayer].SetMyTurn();
            DartOrder[nowPlayer].gameObject.SetActive(true);
        }
    }
}
