using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
    [SerializeField] private int playerIndex = 0;

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

        for(int i =0; i < 2; i++)
        {
            //���� ������ �÷��̾� ����
            PlayerTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<PlayerTokenHandler>();
            //����Ʈ�� �÷��̾� ����
            playerTokenHandlers.Add(handle);
        }

        //handle.curNode = startNode.nextBoard[0];

        //Ʈ����ĭ ����
        //SetTrophyNode();
    }

    //�ӽ��ڵ�
    //public IEnumerator Test()
    //{
    //    yield return GameManager.Instance.InitApp();
    //}

    public void TestRandomDice()
    {
        //�ֻ��� ����
        int rand = Random.Range(1, 7);

        //���� �ֻ����� ���� �÷��̾ �Է�
        Curplayer.GetDice(rand);
    }

    public void TestTurnEnd()
    {
        if(Curplayer.IsTurnEnd())
        {
            //(�����ο� + 1) % (�����ο� + 1)
            int count = playerTokenHandlers.Count;
            playerIndex = (playerIndex + 1) % (count + 1);
            //playerIndex++;
        }
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
