using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

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
    public List<BoardTokenHandler> playerTokenHandlers = new();
    public Material[] materials;

    //���� ���� �÷��̾� �ε���
    [SerializeField] private int playerIndex = 0;

    public List<IToggle> trophyNode = new List<IToggle>();
    public List<AreaNode> areaNodes = new List<AreaNode>();
    private int prevTrophyIndex = -1;
    public event Action OnEvent;

    private List<IGameResult> bonus;

    public BoardTokenHandler Curplayer
    {
        get { return playerTokenHandlers[playerIndex]; }
    }

    public int curPlayerIndex
    {
        get { return playerIndex; }
    }

    protected override void Awake()
    {
        base.Awake();
        isDontDestroyOnLoad = false;

        //�׽�Ʈ��
        //StartCoroutine(Init());

        Init();

        //Ʈ����ĭ ����
        SetTrophyNode();
        SetBonus();
    }

    private void Init()
    {
        int count = GameManager.Instance.SessionDic.Count;

        for (int i = 0; i < count; i++)
        {
            //���� ������ �÷��̾� ����
            BoardTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<BoardTokenHandler>();

            if (GameManager.Instance.myInfo.Color == i)
            {
                handle.isMine = true;
                handle.SetColor(i);
            }

            //����Ʈ�� �÷��̾� ����
            playerTokenHandlers.Add(handle);
        }

        Curplayer.Ready();
    }

    //�׽�Ʈ��
    //private IEnumerator Init()
    //{
    //    //yield return new WaitUntil(() => GameManager.Instance.isInitialized);

    //    for(int i =0; i < 2; i++)
    //    {
    //        //���� ������ �÷��̾� ����
    //        BoardTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<BoardTokenHandler>();
    //        //handle.data.
    //        //����Ʈ�� �÷��̾� ����
    //        playerTokenHandlers.Add(handle);
    //    }

    //    Curplayer.Ready();
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
            int count = playerTokenHandlers.Count;
            playerIndex = (playerIndex + 1) % (count);
            Curplayer.Ready();

            //�̴ϰ��� ����
            //OnEvent?.Invoke();

            //��������
            //GameOver();
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

    public void PurChaseNode(int node,int playerIndex)
    {
        areaNodes[node].SetArea(playerIndex);
    }
    
    private void SetBonus()
    {
        bonus = new();
        List<int> num = new();
        
        for(int i = 0; i < 3;)
        {
            int rand = UnityEngine.Random.Range(0, 13);

            if (num.Contains(rand)) continue;
            num.Add(rand);
            //***���� �������ÿ�, ��¥ �������
            switch(rand)
            {
                case 0:
                    bonus.Add(new FastCoinZero());
                    break;
                case 1:
                    bonus.Add(new HighComebackCount());
                    break;
                case 2:
                    bonus.Add(new HighDiceCount());
                    break;
                case 3:
                    bonus.Add(new HighPaymentCount());
                    break;
                case 4:
                    bonus.Add(new HighPurchaseCount());
                    break;
                case 5:
                    bonus.Add(new HighSaveCoin());
                    break;
                case 6:
                    bonus.Add(new HighSellCount());
                    break;
                case 7:
                    bonus.Add(new HighTaxCount());
                    break;
                case 8:
                    bonus.Add(new LoseCount());
                    break;
                case 9:
                    bonus.Add(new LowDiceCount());
                    break;
                case 10:
                    bonus.Add(new LowPurchaseCount());
                    break;
                case 11:
                    bonus.Add(new NoneTrophy());
                    break;
                case 12:
                    bonus.Add(new WinCount());
                    break;
            }

            i++;
        }
    }

    public async void GameOver()
    {
        //��������� ��ũ�����̼�, �߰� Ʈ���� ����
        foreach (var result in bonus)
        {
            List<int> list = result.Result();

            foreach (int i in list)
                playerTokenHandlers[i].data.trophyAmount += 1;
        }

        //�������� �ε��� ����
        playerTokenHandlers.Sort((a,b) => 
        {
            if(a.data.trophyAmount == b.data.trophyAmount)
                return b.data.keyAmount.CompareTo(a.data.keyAmount);

            return b.data.trophyAmount.CompareTo(a.data.trophyAmount);
        });

        await UIManager.Show<BoardResult>();
    }
}
