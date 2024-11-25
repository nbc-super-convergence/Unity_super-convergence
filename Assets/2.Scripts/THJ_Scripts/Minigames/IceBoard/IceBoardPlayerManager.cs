using System.Collections.Generic;
using UnityEngine;

public class IceBoardPlayerManager : MonoBehaviour
{
    private static IceBoardPlayerManager instance;

    public static IceBoardPlayerManager Instance
    {
        get => instance;
        set
        {
            if (instance == null) instance = value;
        }
    }


    //안에 플레이어들의 위치와 회전값 그리고 아이디를 받아서 각 위치값을 반영
    [SerializeField] private List<Player> multiPlayers;
    private GamePacket gamePacket = new();

    public int CurrentId { get; private set; } = 1;  //유저의 현재 아이디 (서버에서 가져올 거임)

    private void Awake()
    {
        Instance = this;

        multiPlayers = new List<Player>(4);

        //전체 플레이어에서 유저 등록(?)
        Player _player;
        for (int i = 0; i < transform.childCount; i++)
        {
            _player = transform.GetChild(i).GetComponent<Player>();
            _player.CurrentId = (i + 1);
            multiPlayers.Add(_player);
        }


        CurrentId = GameManager.Instance.GetPlayerId();
    }

    private void Start()
    {
        //내가 지금 몇P인지
        for (int i = 0; i < multiPlayers.Count; i++)
        {
            if (multiPlayers[i].CurrentId == CurrentId)
                multiPlayers[i].EnablePlayer();
        }
    }

    private void FixedUpdate()
    {

    }

    public void ReceivePosition(S2C_IcePlayerMoveNotification response)
    {
        //SocketManager에서 각 플레이어의Receive메서드로 받아서 전달
        //for (int i = 0; i < multiPlayers.Count; i++)
        //{
        //    if (CurrentId != (i + 1))  //내가 아닌 상대라면
        //    {
        //        multiPlayers[i].ReceivePosition(gamePacket);    //패킷에서 데이터 받기
        //    }
        //}

        foreach (var playerData in response.Players)
        {
            var player = multiPlayers.Find(obj => obj.CurrentId == playerData.PlayerId);
            player.ReceivePosition(response);
        }
    }

    public void SpawnPosition(S2C_IcePlayerSpawnNotification response)
    {
        int CurrentId = response.PlayerId;

        Debug.Log(response);
        foreach(Player player in multiPlayers)
            player.ReceivePosition(response);
    }
}