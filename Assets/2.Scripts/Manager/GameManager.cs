using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //로그인 -> 다른 씬으로 이동 시.

    public IEnumerator InitApp()
    {
        yield return InitManagers();

        //Initialize GameManager : 확실한 초기화 보장.
        isInitialized = true;
    }

    private IEnumerator InitManagers()
    {
        //Initialize ResourceManager
        ResourceManager.Instance.Init();
        yield return new WaitUntil(() => ResourceManager.Instance.isInitialized);

        //Initialize UIManager
        UIManager.Instance.Init();
        yield return new WaitUntil(() => UIManager.Instance.isInitialized);
    }
}