using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : Singleton<MiniGameManager>
{
    #region 변수목록
    MiniGameData gameData;
    #endregion

    #region Unity메소드
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
    #endregion

    #region 메인 메서드
    public virtual void GameStart()
    {

    }

    public virtual void GameStop() 
    { 
    
    }
   
    public virtual void GameEnd()
    {

    }
    #endregion

    #region 서브 메서드
    //초기화 작업 - kh
    public virtual void Init()
    {
        isDontDestroyOnLoad = false;
    }

    //현재 점수 변경 - kh
    public void ChangeScore(int _score)
    {

    }

    //현재 시간 변경 - kh
    public void ChangeTime(float _time)
    {

    }

    //미니게임 맵 설정 - kh
    public void OnChangeMap()
    {

    }

    //배경음 설정 - kh
    public void OnChangedBGM()
    {

    }
    #endregion
}
