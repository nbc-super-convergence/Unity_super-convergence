using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBoardPlayerManager : MonoBehaviour
{
    //�ȿ� �÷��̾���� ��ġ�� ȸ���� �׸��� ���̵� �޾Ƽ� �� ��ġ���� �ݿ�
    List<Player> multiPlayers;
    GamePacket gamePacket = new ();

    public int CurrentId { get; private set; } = 1;  //������ ���� ���̵�

    private void Awake()
    {
        multiPlayers = new List<Player>(4);

        //��ü �÷��̾�� ���� ���(?)
        Player _player;
        for (int i = 0; i < transform.childCount; i++)
        {
            _player = transform.GetChild(i).GetComponent<Player>();

            if (CurrentId == (i + 1))   //���� ���̵� �ݿ�
                _player.CurrentId = (i + 1);

            multiPlayers.Add(_player);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < multiPlayers.Count; i++)
        {
            if (CurrentId != i) //���� ���̵� �ƴ϶�� �ٸ� ������� ��ġ�� ������ �ݿ�
            {
                //multiPlayers[i].transform = �������� �������� �ݿ�
            } 
        }
    }

    private void ReceivePosition(GamePacket receivePacket)  //�������� �޾ƿ� ��ġ���� �ݿ� (�������)
    {

    }
}
