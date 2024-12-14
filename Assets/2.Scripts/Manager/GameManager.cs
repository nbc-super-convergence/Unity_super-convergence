using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //BoardScene으로 넘어갈 때???
    public static Action<int> OnPlayerLeft; //색깔 전달

    public UserInfo myInfo = new();

    //0:빨강, 1:노랑, 2:초록, 3:파랑
    public Dictionary<string, UserInfo> SessionDic { get; private set; } = new();
    public Dictionary<int, string> failCodeDic;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

    public void InitApp()
    {
        StartCoroutine(InitManagers());
        
    }

    private IEnumerator InitManagers()
    {
        //Initialize ResourceManager
        StartCoroutine(ResourceManager.Instance.Init());
        yield return new WaitUntil(() => ResourceManager.Instance.isInitialized);

        //Initialize UIManager
        UIManager.Instance.Init();
        yield return new WaitUntil(() => UIManager.Instance.isInitialized);

        CSVParser.Instance.Init();
        yield return new WaitUntil(() => CSVParser.Instance.isInitialized);

        PoolManager.Instance.Init();
        yield return new WaitUntil(() => PoolManager.Instance.isInitialized);


        //Initialize SocketManager
        SocketManager.Instance.Init();

        //Initialize GameManager : 확실한 초기화 보장.
        isInitialized = true;
    }

    #region SessionDic
    public void AddNewPlayer(string sessionId, string nickname, int color, int order)
    {
        SessionDic.Add(sessionId, new UserInfo(sessionId, nickname, color, order));
    }

    public void DeleteSessionId(string sessionId)
    {
        OnPlayerLeft?.Invoke(SessionDic[sessionId].Color);
        SessionDic.Remove(sessionId);
    }

    public string FindSessionIdByColor(int color)
    {
        var pair = SessionDic.FirstOrDefault(x => x.Value.Color == color);
        return pair.Key != null ? pair.Key : null;
    }
    #endregion
}
