using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBoardPlayerManager : MonoBehaviour
{
    //안에 플레이어들의 위치와 회전값 그리고 아이디를 받아서 각 위치값을 반영
    List<Player> multiPlayers;
    GamePacket gamePacket = new ();

    public int CurrentId { get; private set; } = 1;  //유저의 현재 아이디

    private void Awake()
    {
        multiPlayers = new List<Player>(4);

        //전체 플레이어에서 유저 등록(?)
        Player _player;
        for (int i = 0; i < transform.childCount; i++)
        {
            _player = transform.GetChild(i).GetComponent<Player>();

            if (CurrentId == (i + 1))   //유저 아이디 반영
                _player.CurrentId = (i + 1);

            multiPlayers.Add(_player);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < multiPlayers.Count; i++)
        {
            if (CurrentId != i) //현재 아이디가 아니라면 다른 사용자의 위치의 정보를 반영
            {
                //multiPlayers[i].transform = 서버에서 받은값을 반영
            } 
        }
    }

    private void ReceivePosition(GamePacket receivePacket)  //서버에서 받아온 위치값을 반영 (상대팀들)
    {

    }
}
