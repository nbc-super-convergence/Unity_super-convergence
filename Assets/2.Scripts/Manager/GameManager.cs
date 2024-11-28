using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //로그인 -> 다른 씬으로 이동 시.
    //public int PlayerId { get; private set; }
    public int PlayerId;

    public UserInfo myInfo = new();

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

        //Initialize GameManager : 확실한 초기화 보장.
        isInitialized = true;
    }

    #region Client ID
    //public void SetPlayerId(int playerId)
    //{
    //    PlayerId = playerId;
    //}
    #endregion
}
