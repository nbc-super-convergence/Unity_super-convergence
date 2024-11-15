using System.Collections.Generic;
using System.Collections;
using UnityEngine;

//���� �׽�Ʈ �ڵ�
public class MapControl : Singleton<MapControl>
{
    //���� ����
    public Transform startNode;

    // �׽�Ʈ �÷��̾� ������
    public GameObject TestPlayerPrefab;

    //�÷��̾� ����Ʈ
    public List<PlayerTokenHandler> playerTokenHandlers = new();
    

    public Material[] materials;

    //���� ���� �÷��̾� �ε���
    private int playerIndex = 0;

    public PlayerTokenHandler Curplayer
    {
        get { return playerTokenHandlers[playerIndex]; }
    }

    protected override void Awake()
    {
        //�ӽ��ڵ�
        StartCoroutine(Test());

        base.Awake();
        isDontDestroyOnLoad = false;

        //���� ������ �÷��̾� ����
        PlayerTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<PlayerTokenHandler>();
        //����Ʈ�� �÷��̾� ����
        playerTokenHandlers.Add(handle);
        //handle.curNode = startNode.nextBoard[0];
    }

    //�ӽ��ڵ�
    public IEnumerator Test()
    {
        yield return ResourceManager.Instance.Init();
    }

    public void TestRandomDice()
    {
        //�ֻ��� ����
        int rand = Random.Range(1, 7);
        //���� �ֻ����� ���� �÷��̾ �Է�
        playerTokenHandlers[playerIndex].GetDice(3);
    }

    public void TestTurnEnd()
    {
        playerIndex++;
    }

  
}
