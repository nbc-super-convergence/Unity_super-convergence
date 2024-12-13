using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

#region 서버연결
///// <summary>
///// C2S_RollDiceRequest 요청패킷 전송.
///// GamePacket packet = new();
///// packet.RollDiceRequest = new()
///// {
/////     PlayerId = GameManager.Instance.GetPlayerId()
///// };
///// SocketManager.Instance.OnSend(packet);
///// S2C_RollDiceResponse 리스폰스 패킷 올때까지 대기. (WaitUntil or Task...)
///// S2C_RollDiceResponse 리스폰스를 성공적으로 받으면 if(response.Success) { Curplayer.GetDice(response.DiceResult); }
///// </summary>
//public void TestRandomDice()
//{
//    //주사위 돌림
//    int rand = Random.Range(1, 7);

//    //나온 주사위의 수를 플레이어에 입력
//    Curplayer.GetDice(rand);
//}

///// <summary>
///// 서버에서는 주사위 돌린 유저에게 S2C_RollDiceResponse를 보내주고 같은 값을 다른 유저에게 S2C_RollDiceNotification을 보낸다.
///// SocketManager에 선언하는 메서드. S2C_RollDiceNotification을 받으면 SocketManager가 자동으로 RollDiceNotification에 대응하는 메서드를 찾아 호출한다.
///// 
///// </summary>
//public void RollDiceNotification(GamePacket gamePacket)
//{
//    var response = gamePacket.RollDiceNotification;
//    int playerId = response.PlayerId;
//    int diceResult = response.DiceResult;

//    // S2C_RollDiceNotification을 받으면 실행할 코드를 여기에 작성한다.
//    // 자기 턴이 아닌 유저에게 지금 턴 유저의 주사위굴림 결과를 보여준다.
//    // BoardManager.Instance.현재턴주사위굴림보여주는메서드이름(diceResult);
//}

#endregion

public class BoardManager : Singleton<BoardManager>
{
    //시작 지점
    public Transform startNode;

    // 테스트 플레이어 프리펩
    public GameObject TestPlayerPrefab;

    //플레이어 리스트
    public List<BoardTokenHandler> playerTokenHandlers = new();
    public Material[] materials;

    //현재 턴의 플레이어 인덱스
    [SerializeField] private int playerIndex = 0;

    public List<IToggle> trophyNode = new List<IToggle>();
    public List<AreaNode> areaNodes = new List<AreaNode>();

    public Dice dice { get; private set; }

#pragma warning disable
    public CinemachineVirtualCamera camera;
    public event Action OnEvent;
#pragma warning restore

    //private List<IGameResult> bonus;
    public bool isMIniPlay { get; private set; }


    public BoardTokenHandler GetToken(string sessionID)
    {
        string id = sessionID;
        var user = GameManager.Instance.SessionDic[id];
        int i = user.Order;

        var player = playerTokenHandlers.Find((obj) => obj.data.userInfo == user);

        //TODO 순위 정하는 게임의 부재로 Order 인덱스가 이상한 이유로 주석처리
        //return playerTokenHandlers[i]; 
        return player;
    }

    public BoardTokenHandler Curplayer
    {
        get { return playerTokenHandlers[playerIndex]; }
    }

    public int curPlayerIndex
    {
        get { return playerIndex; }
    }

    //protected override void Awake()
    //{
    //    base.Awake();

    //    //테스트용
    //    //StartCoroutine(Init());


    //    //트로피칸 설정
    //    //SetTrophyNode();
    //    //SetBonus();
    //}

    private void Start()
    {
        Init();
    }

    private async void Init()
    {
        var ids = GameManager.Instance.SessionDic.Keys;
        var dicePrefab = await ResourceManager.Instance.LoadAsset<Dice>("dice", eAddressableType.Prefab);
        dice = Instantiate(dicePrefab, Vector3.zero, Quaternion.identity);
        dice.gameObject.SetActive(false);

        foreach (string key in ids)
        {
            var dict = GameManager.Instance.SessionDic;
            var info = dict[key];

            BoardTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<BoardTokenHandler>();
            handle.Init(info);
            //handle.data.userInfo = info;
            handle.SetColor(info.Color);
            handle.gameObject.name = key;

            if (key == GameManager.Instance.myInfo.SessionId) handle.isMine = true;

            playerTokenHandlers.Add(handle);
        }
        camera.Follow = camera.LookAt = playerTokenHandlers[playerIndex].transform;

        await UIManager.Show<BoardUI>();
        #region Old
        //for (int i = 0; i < count; i++)
        //{
        //    //시작 지점에 플레이어 생성
        //    BoardTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<BoardTokenHandler>();

        //    if (GameManager.Instance.myInfo.Color == i) handle.isMine = true;

        //    handle.SetColor(i);

        //    //GameManager.Instance.

        //    //리스트에 플레이어 보관
        //    playerTokenHandlers.Add(handle);
        //}
        #endregion
        dice.SetDicePosition(playerTokenHandlers[playerIndex].transform);

        Curplayer.Ready();
        GameOver();
    }

    //테스트용
    //private IEnumerator Init()
    //{
    //    //yield return new WaitUntil(() => GameManager.Instance.isInitialized);

    //    for(int i =0; i < 2; i++)
    //    {
    //        //시작 지점에 플레이어 생성
    //        BoardTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<BoardTokenHandler>();
    //        //handle.data.
    //        //리스트에 플레이어 보관
    //        playerTokenHandlers.Add(handle);
    //    }

    //    Curplayer.Ready();
    //}

    //public void RandomDice()
    //{
    //    #region Old
    //    ////주사위 돌림
    //    //int rand = Random.Range(1, 7);

    //    ////나온 주사위의 수를 플레이어에 입력
    //    //Curplayer.GetDice(rand);
    //    #endregion
    //}

    public void TurnEnd()
    {
        if(Curplayer.IsTurnEnd())
        {

            //if(playerIndex + 1 == playerTokenHandlers.Count)
            //{
            //    int count = playerTokenHandlers.Count;

            //    GamePacket packet = new();
            //    packet.StartMiniGameRequest = new()
            //    {
            //        SessionId = GameManager.Instance.myInfo.SessionId,
            //    };

            //    SocketManager.Instance.OnSend(packet);
            //    UIManager.Hide<BoardUI>();

            //    Debug.Log("StartMiniGameRequest");
            //}
            //else
            //{
            GamePacket packet = new();

            packet.TurnEndRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId
            };
                
            SocketManager.Instance.OnSend(packet);

            NextTurn();
            //}

            #region Old

            //(현재인원 + 1) % (현재인원 + 1)
            //int count = playerTokenHandlers.Count;
            //playerIndex = (playerIndex + 1) % (count);
            //Curplayer.Ready();

            //미니게임 시작
            //OnEvent?.Invoke();

            //게임종료
            //GameOver();
            #endregion
        }
    }
    public void NextTurn()
    {
        isMIniPlay = false;

        int count = playerTokenHandlers.Count;
        playerIndex = (playerIndex + 1) % count;

        Transform t = playerTokenHandlers[playerIndex].transform;
        camera.Follow = camera.LookAt = t;
        dice.SetDicePosition(t);

        Curplayer.Ready();
    }

    //public void SetTrophyNode()
    //{
    //    //GamePacket packet = new();

    //    ////packet.() = new()
    //    ////{

    //    ////};

    //    //SocketManager.Instance.OnSend(packet);

    //    #region Old
    //    //int rand = Random.Range(0, trophyNode.Count);

    //    //while(rand == prevTrophyIndex)
    //    //    rand = Random.Range(0, trophyNode.Count);

    //    //if(prevTrophyIndex != -1)
    //    //    trophyNode[prevTrophyIndex].Toggle();

    //    //trophyNode[rand].Toggle();

    //    //prevTrophyIndex = rand;
    //    #endregion
    //}

    public void StartMinigame()
    {
        isMIniPlay = true;

        GamePacket packet = new();

        packet.StartMiniGameRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
        };

        SocketManager.Instance.OnSend(packet);
    }

    //public void PurChaseNode(int node,int playerIndex)
    //{
    //    areaNodes[node].SetArea(playerIndex);
    //}

    //private void SetBonus()
    //{
    //    bonus = new();
    //    List<int> num = new();
        
    //    for(int i = 0; i < 3;)
    //    {
    //        int rand = UnityEngine.Random.Range(0, 13);

    //        if (num.Contains(rand)) continue;
    //        num.Add(rand);
    //        //***주의 열지마시오, 진짜 경고했음
    //        switch(rand)
    //        {
    //            case 0:
    //                bonus.Add(new FastCoinZero());
    //                break;
    //            case 1:
    //                bonus.Add(new HighComebackCount());
    //                break;
    //            case 2:
    //                bonus.Add(new HighDiceCount());
    //                break;
    //            case 3:
    //                bonus.Add(new HighPaymentCount());
    //                break;
    //            case 4:
    //                bonus.Add(new HighPurchaseCount());
    //                break;
    //            case 5:
    //                bonus.Add(new HighSaveCoin());
    //                break;
    //            case 6:
    //                bonus.Add(new HighSellCount());
    //                break;
    //            case 7:
    //                bonus.Add(new HighTaxCount());
    //                break;
    //            case 8:
    //                bonus.Add(new LoseCount());
    //                break;
    //            case 9:
    //                bonus.Add(new LowDiceCount());
    //                break;
    //            case 10:
    //                bonus.Add(new LowPurchaseCount());
    //                break;
    //            case 11:
    //                bonus.Add(new NoneTrophy());
    //                break;
    //            case 12:
    //                bonus.Add(new WinCount());
    //                break;
    //        }

    //        i++;
    //    }
    //}

    public async void GameOver()
    {
        //게임종료시 레크리에이션, 추가 트로피 증정
        //foreach (var result in bonus)
        //{
        //    List<int> list = result.Result();

        //    foreach (int i in list)
        //        playerTokenHandlers[i].data.trophyAmount += 1;
        //}

        //순위별로 인덱스 변경
        UIManager.Hide<BoardUI>();

        playerTokenHandlers.Sort((a,b) => 
        {
            //if(a.data.trophyAmount == b.data.trophyAmount)
            return b.data.coin.CompareTo(a.data.coin);

            //return b.data.trophyAmount.CompareTo(a.data.trophyAmount);
        });
        Debug.Log("결과창 생성");
        await UIManager.Show<BoardResultUI>();
    }

    //순서 변경 없음으로 인한 주석처리
    //private void SeqeunceUpdate()
    //{
    //    playerTokenHandlers.Sort((a,b) => { return a.data.userInfo.Order.CompareTo(b.data.userInfo.Order); });
    //}
}
