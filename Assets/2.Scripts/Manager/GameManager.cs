using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //BoardScene���� �Ѿ ��???
    public static Action<int> OnPlayerLeft; //���� ����

    public UserInfo myInfo = new();

    //0:����, 1:���, 2:�ʷ�, 3:�Ķ�
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

        //Initialize SocketManager
        SocketManager.Instance.Init();

        //Initialize GameManager : Ȯ���� �ʱ�ȭ ����.
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
    #endregion
}
