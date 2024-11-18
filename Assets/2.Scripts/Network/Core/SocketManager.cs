using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GamePacket;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    //TODO: ���ο� �Լ� ���� �� private void�� ���� ��.
    //TODO: �Լ��� �̸��� �ݵ�� PayloadOneOfCase Enum�� ���� ��.
    //TODO: ���ڴ� GamePacket gamePacket.

    public void IceJoinRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IceJoinRequest;
    }

    public void IceStartRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IceStartRequest;
    }

    public void IcePlayerMoveRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveRequest;
    }

    //�ٸ� �÷��̾� ������
    public void IceMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMoveNotification;
        
    }

    //�ٸ� �÷��̾� ����
    public void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;
    }

    //���� ���� (���߿� ����)
    public void IceStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceStartNotification;
    }

    //�ִϸ��̼� �ݿ�
    public void IcePlayersStateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayersStateSyncNotification;
    }

    //�÷��̾� ��� �̺�Ʈ
    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDeathNotification;
    }

    //�� ���� �̺�Ʈ(���� ������ ��ȿ��, ������ ����)
    public void IceMapStateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMapStateSyncNotification;
    }

    //Ice �̴ϰ��� ����
    public void IceOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceOverNotification;
    }

}