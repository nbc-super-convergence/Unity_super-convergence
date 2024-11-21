using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class IceBoardPlayerManager : MonoBehaviour
{
    //안에 플레이어들의 위치와 회전값 그리고 아이디를 받아서 각 위치값을 반영
    [SerializeField] private List<Player> multiPlayers;
    private GamePacket gamePacket = new ();

    public int CurrentId { get; private set; } = 4;  //유저의 현재 아이디 (서버에서 가져올 거임)

    private void Awake()
    {
        multiPlayers = new List<Player>(4);

        //전체 플레이어에서 유저 등록(?)
        Player _player;
        for (int i = 0; i < transform.childCount; i++)
        {
            _player = transform.GetChild(i).GetComponent<Player>();
            _player.CurrentId = (i + 1);
            multiPlayers.Add(_player);
        }
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
}
