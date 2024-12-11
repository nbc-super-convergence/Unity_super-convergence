using System.Collections.Generic;
using UnityEngine;

public class MapGameDart : MapBase
{
    [SerializeField] private SelectOrderPanel DartPannel;

    //다트그룹
    public List<SelectOrderDart> DartOrder;

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
        }
    }

    /// <summary>
    /// 지금부터 시작
    /// </summary>
    private void BeginSelectOrder()
    {
        DartOrder[nowPlayer].gameObject.SetActive(true);
    }
}