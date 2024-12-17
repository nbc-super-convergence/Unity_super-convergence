using System;
using System.Threading.Tasks;
using UnityEngine;

public enum eGameType
{//실존 클래스명과 일치해야 함. 서순 절대 바꾸지 말 것.
    Default,
    GameIceSlider,
    GameBombDelivery,
    GameCourtshipDance,
    GameDropper,
    GameDart
}

public class MinigameManager : Singleton<MinigameManager>
{
    #region Field
    public GameObject boardCamera; //보드게임 카메라
    [SerializeField] private Transform miniParent; //hiearchy 부모
    
    public static eGameType gameType { get; private set; } //게임 종류 enum
    public IGame curMiniGame; //특정 미니게임 한정 기능구현
    public MapBase curMap; //미니게임 맵 오브젝트 관련 기능구현

    public MiniToken[] miniTokens; //미니게임 캐릭터
    public string mySessonId => GameManager.Instance.myInfo.SessionId;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnPlayerLeft += PlayerLeftEvent;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
    }

    #region Properties
    /// <summary>
    /// curMiniGame T로 변환
    /// </summary>
    public T GetMiniGame<T>() where T : IGame
    {
        return (T)curMiniGame;
    }

    /// <summary>
    /// sessionId에 맞는 미니토큰
    /// </summary>
    public MiniToken GetMiniToken(string sessionId)
    {
        if (GameManager.Instance.SessionDic.TryGetValue(sessionId, out UserInfo info))
            return miniTokens[info.Color];
        else
            return null;
    }

    /// <summary>
    /// 클라 미니토큰
    /// </summary>
    public MiniToken GetMyToken()
    {
        int idx = GameManager.Instance.SessionDic[mySessonId].Color;
        return miniTokens[idx];
    }

    /// <summary>
    /// curMap T로 변환
    /// </summary>
    public async Task<T> GetMap<T>() where T : MapBase
    {
        // curMap이 null이면 반복적으로 검사하며 대기
        while (curMap == null)
        {
            await Task.Yield(); // 다음 프레임까지 비동기적으로 대기
        }

        return (T)curMap;
    }
    #endregion

    #region Minigame 초기화
    /// <summary>
    /// 서버에서 정한 미니게임 선택 및 초기화
    /// </summary>
    /// <typeparam name="T">IGame의 자식 클래스</typeparam>
    public T SetMiniGame<T>(params object[] param) where T : IGame, new()
    {
        gameType = (eGameType)Enum.Parse(typeof(eGameType), typeof(T).Name);
        curMiniGame = new T();
        curMiniGame.Init(param);
        
        return (T)curMiniGame;
    }

    //미니게임 맵 생성
    public void MakeMap<T>() where T : MapBase
    {
         GameObject instantiatedMap = Instantiate(curMap.gameObject, miniParent);
        curMap = instantiatedMap.GetComponent<T>();
    }

    public async Task MakeMapDance()
    {
        GameObject instantiatedMap = Instantiate(curMap.gameObject, miniParent);
        await Task.Yield();
        curMap = instantiatedMap.GetComponent<MapGameCourtshipDance>();
    }
    #endregion

    private void PlayerLeftEvent(int color)
    {
        miniTokens[color].gameObject.SetActive(false);
    }
}