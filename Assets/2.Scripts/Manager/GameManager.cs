using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //BoardScene���� �Ѿ ��???
    
    public UserInfo myInfo = new();

    //0:����, 1:���, 2:�ʷ�, 3:�Ķ�
    public Dictionary<string, int> SessionDic { get; private set; } = new();

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

        //Initialize SocketManager
        SocketManager.Instance.Init();

        //Initialize GameManager : Ȯ���� �ʱ�ȭ ����.
        isInitialized = true;
    }

    #region Client ID
    public void SetPlayerId(string sessionId, int playerId)
    {
        SessionDic[sessionId] = playerId;
    }

    public void DeletePlayerId(string sessionId)
    {
        MinigameManager.Instance.GetMiniToken(sessionId).gameObject.SetActive(false);
        SessionDic[sessionId] = -1;
    }
    #endregion
}
