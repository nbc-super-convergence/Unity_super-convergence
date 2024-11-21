using System.Collections.Generic;
using UnityEngine;

//���� �׽�Ʈ �ڵ�
public class BoardManager : Singleton<BoardManager>
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

    public List<IToggle> trophyNode = new List<IToggle>();
    private int prevTrophyIndex = -1;

    public PlayerTokenHandler Curplayer
    {
        get { return playerTokenHandlers[playerIndex]; }
    }

    public int curPlayerIndex
    {
        get { return playerIndex; }
    }

    protected override void Awake()
    {
        //�ӽ��ڵ�
        //StartCoroutine(Test());

        base.Awake();
        isDontDestroyOnLoad = false;

        //���� ������ �÷��̾� ����
        PlayerTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<PlayerTokenHandler>();
        //����Ʈ�� �÷��̾� ����
        playerTokenHandlers.Add(handle);
        //handle.curNode = startNode.nextBoard[0];
    }

    //�ӽ��ڵ�
    //public IEnumerator Test()
    //{
    //    yield return ResourceManager.Instance.Init();
    //}

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

    public void SetTrophyNode()
    {
        int rand = Random.Range(0, trophyNode.Count);

        while(rand == prevTrophyIndex)
            rand = Random.Range(0, trophyNode.Count);

        if(prevTrophyIndex != -1)
            trophyNode[prevTrophyIndex].Toggle();

        trophyNode[rand].Toggle();

        prevTrophyIndex = rand;
    }
}