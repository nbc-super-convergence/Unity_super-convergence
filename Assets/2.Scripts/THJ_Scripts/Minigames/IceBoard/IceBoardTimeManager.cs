﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IceBoardTimeManager : MonoBehaviour
{
    /// <summary>
    /// 시간 : 120초
    /// 면적 : 기본 15 -> 10 -> 5
    /// 90초 이하일 때 10
    /// 60초 이하일 때 5
    /// </summary>
    /// 

    private static IceBoardTimeManager _instance; //싱글톤 (이유는 플레이어와 연동하게)
    public static IceBoardTimeManager Instance 
    { 
        get
        {
            return _instance;
        }
        private set
        {
            if (_instance == null)
                _instance = value;
        }
    }

    //기본 속성
    private float _startSecond = 120f;
    private float _currentSecond;   //이 속성을 패킷에 보내야함
    public bool GameOver { get; private set; } = false;

    //UI 속성
    [SerializeField] private TextMeshProUGUI SecondTextUI;
    [SerializeField] private TextMeshProUGUI GameSetTextUI;

    //빙판 속성
    [SerializeField] private IceBoardFloor iceBoardFloor;

    private void Awake()
    {
        Instance = this;    //싱글톤 정의
    }

    void Start()
    {
        _currentSecond = _startSecond;  //시작할 때 시간을 초기
    }

    void Update()
    {
        if (!GameOver)
        {
            _currentSecond -= Time.deltaTime;   //시간 갱신
            TimeFlag((int)_currentSecond);
        }
        ApplyText((int)_currentSecond);
        ApplyGameSet();
    }

    /// <summary>
    /// 일정 시간이 경과하면 해당 이벤트 발생
    /// </summary>
    /// <param name="sec"></param>
    private void TimeFlag(int sec)
    {
        if (sec == 0)
        {
            GameOver = true;
            return;
        }
        else if(sec <= 60)
        {
            iceBoardFloor.SetLength(5);
        }    
        else if(sec <= 90)
        {
            iceBoardFloor.SetLength(10);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 초가 바뀌면 텍스트 갱신
    /// </summary>
    /// <param name="sec">텍스트에 적용할 초</param>
    private void ApplyText(int sec)
    {
        SecondTextUI.text = $"Time : {sec.ToString()}";
    }

    /// <summary>
    /// 게임오버 표시
    /// </summary>
    private void ApplyGameSet()
    {
        GameSetTextUI.gameObject.SetActive(GameOver);
    }
}
 