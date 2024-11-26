using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //�α��� -> �ٸ� ������ �̵� ��.
    public int PlayerId { get; private set; }

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

        //Initialize GameManager : Ȯ���� �ʱ�ȭ ����.
        isInitialized = true;
    }

    #region Client ID
    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }
    #endregion
}