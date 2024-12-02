using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IceBoardTimeManager : Singleton<IceBoardTimeManager>
{
    /// <summary>
    /// 시간 : 120초
    /// 면적 : 기본 15 -> 10 -> 5
    /// 90초 이하일 때 10
    /// 60초 이하일 때 5
    /// </summary>
    /// 

    //기본 속성
    private float _startSecond = 120f;
    private float _currentSecond;   //이 속성을 패킷에 보내야함
    public bool GameOver { get; private set; } = false;

    //UI 속성
    [SerializeField] private TextMeshProUGUI SecondTextUI;
    [SerializeField] private TextMeshProUGUI GameSetTextUI;

    //빙판 속성
    [SerializeField] private IceBoardFloor iceBoardFloor;

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
            GameOver = true;    //끝났다고 서버에 전송
            return;
        }
        else if(sec <= 60)  //아래 데이터들을 서버에 전송
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
 