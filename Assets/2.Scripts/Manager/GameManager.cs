using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isGameStart; //�α��� -> �ٸ� ������ �̵� ��.

    public IEnumerator InitApp()
    {
        yield return InitManagers();

        //Initialize GameManager : Ȯ���� �ʱ�ȭ ����.
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