using System;
using UnityEngine;

public enum eGameType
{//실존 클래스명과 일치해야 함. 서순 절대 바꾸지 말 것.
    Default,
    GameIceSlider,
    GameBombDelivery
}

public class MinigameManager : Singleton<MinigameManager>
{
    [SerializeField] private Transform MiniParent; //미니게임 오브젝트 부모
    public GameObject boardCamera; //보드게임 카메라
    public MapBase CurMap;

    /*미니게임 정보*/
    public static eGameType GameType { get; private set; } //게임 종류
    private IGame curMiniGame; //미니게임 관련 메서드 호출용

    public MiniToken[] MiniTokens; //미니게임 캐릭터
    public string MySessonId => GameManager.Instance.myInfo.SessionId;

    #region Properties
    public T GetMiniGame<T>() where T : IGame
    {
        return (T)curMiniGame;
    }

    public MiniToken GetMiniToken(string sessionId)
    {
        if (GameManager.Instance.SessionDic.TryGetValue(sessionId, out UserInfo idx))
            return MiniTokens[idx.Color];
        else
            return null;
    }

    public MiniToken GetMyToken()
    {
        int idx = GameManager.Instance.SessionDic[MySessonId].Color;
        return MiniTokens[idx];
    }

    public T GetMap<T>() where T : MapBase
    {
        return (T)CurMap;
    }
    #endregion

    #region Minigame 초기화D
    /// <summary>
    /// 서버에서 정한 미니게임 선택.
    /// </summary>
    /// <typeparam name="T">IGame의 자식 클래스</typeparam>
    public T SetMiniGame<T>(params object[] param) where T : IGame, new()
    {
        GameType = (eGameType)Enum.Parse(typeof(eGameType), typeof(T).Name);
        curMiniGame = new T();
        curMiniGame.Init(param);
        
        return (T)curMiniGame;
    }

    //미니게임 맵 설정
    public void MakeMap()
    {
        Instantiate(CurMap.gameObject, MiniParent);
    }
    #endregion
}