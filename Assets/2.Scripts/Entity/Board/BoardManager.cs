using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

#region ��������
///// <summary>
///// C2S_RollDiceRequest ��û��Ŷ ����.
///// GamePacket packet = new();
///// packet.RollDiceRequest = new()
///// {
/////     PlayerId = GameManager.Instance.GetPlayerId()
///// };
///// SocketManager.Instance.OnSend(packet);
///// S2C_RollDiceResponse �������� ��Ŷ �ö����� ���. (WaitUntil or Task...)
///// S2C_RollDiceResponse ���������� ���������� ������ if(response.Success) { Curplayer.GetDice(response.DiceResult); }
///// </summary>
//public void TestRandomDice()
//{
//    //�ֻ��� ����
//    int rand = Random.Range(1, 7);

//    //���� �ֻ����� ���� �÷��̾ �Է�
//    Curplayer.GetDice(rand);
//}

///// <summary>
///// ���������� �ֻ��� ���� �������� S2C_RollDiceResponse�� �����ְ� ���� ���� �ٸ� �������� S2C_RollDiceNotification�� ������.
///// SocketManager�� �����ϴ� �޼���. S2C_RollDiceNotification�� ������ SocketManager�� �ڵ����� RollDiceNotification�� �����ϴ� �޼��带 ã�� ȣ���Ѵ�.
///// 
///// </summary>
//public void RollDiceNotification(GamePacket gamePacket)
//{
//    var response = gamePacket.RollDiceNotification;
//    int playerId = response.PlayerId;
//    int diceResult = response.DiceResult;

//    // S2C_RollDiceNotification�� ������ ������ �ڵ带 ���⿡ �ۼ��Ѵ�.
//    // �ڱ� ���� �ƴ� �������� ���� �� ������ �ֻ������� ����� �����ش�.
//    // BoardManager.Instance.�������ֻ������������ִ¸޼����̸�(diceResult);
//}

#endregion

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
        SetTrophyNode();
    }

    //�ӽ��ڵ�
    //public IEnumerator Test()
    //{
    //    yield return GameManager.Instance.InitApp();
    //}

    public void RandomDice()
    {
        //GamePacket packet = new();

        ////packet.RollDiceReqeust = new()
        ////{
        ////    PlayerId = playerIndex
        ////};

        //SocketManager.Instance.OnSend(packet);

        #region Old
        ////�ֻ��� ����
        //int rand = Random.Range(1, 7);

        ////���� �ֻ����� ���� �÷��̾ �Է�
        //Curplayer.GetDice(rand);
        #endregion
    }

    public void TurnEnd()
    {
        if(Curplayer.IsTurnEnd())
        {
            //GamePacket packet = new();

            ////packet. = new()
            ////{
                    
            ////};

            //SocketManager.Instance.OnSend(packet);

            #region Old
            //(�����ο� + 1) % (�����ο� + 1)
            //int count = playerTokenHandlers.Count;
            //playerIndex = (playerIndex + 1) % (count + 1);
            //playerIndex++;
            #endregion
        }
    }

    public void SetTrophyNode()
    {
        //GamePacket packet = new();

        ////packet.() = new()
        ////{

        ////};

        //SocketManager.Instance.OnSend(packet);

        #region Old
        //int rand = Random.Range(0, trophyNode.Count);

        //while(rand == prevTrophyIndex)
        //    rand = Random.Range(0, trophyNode.Count);

        //if(prevTrophyIndex != -1)
        //    trophyNode[prevTrophyIndex].Toggle();

        //trophyNode[rand].Toggle();

        //prevTrophyIndex = rand;
        #endregion
    }

    private void StartMiniGame()
    {
        //GamePacket packet = new();

        ////packet.MiniGame() = new()
        ////{

        ////};

        //SocketManager.Instance.OnSend(packet);
    }
}
