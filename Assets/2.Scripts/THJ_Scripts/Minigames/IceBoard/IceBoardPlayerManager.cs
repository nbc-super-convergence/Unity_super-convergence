using System.Collections.Generic;
using UnityEngine;

public class IceBoardPlayerManager : MonoBehaviour
{
    //안에 플레이어들의 위치와 회전값 그리고 아이디를 받아서 각 위치값을 반영
    [SerializeField] private List<Player> multiPlayers;
    private GamePacket gamePacket = new ();

    public int CurrentId { get; private set; } = 1;  //유저의 현재 아이디 (서버에서 가져올 거임)

    private void Awake()
    {
        //소켓에서 받아오기
        SocketManager.Instance.Init();

        //서버에 있는 데이터를 임포트
        gamePacket.IcePlayerMoveNotification = new()
        {

        };

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

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        ReceivePosition();
    }

    private void ReceivePosition()
    {
        //SocketManager에서 각 플레이어의Receive메서드로 받아서 전달
        for (int i = 0; i < multiPlayers.Count; i++)
        {
            if (CurrentId != (i + 1))  //내가 아닌 상대라면
            {
                multiPlayers[i].ReceivePosition(gamePacket);    //패킷에서 데이터 받기
            }
        }
    }
}
