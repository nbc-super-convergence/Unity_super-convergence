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

    //�ٸ� �÷��̾� ������
    private void IceMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMoveNotification;
        
    }

    //�ٸ� �÷��̾� ����
    private void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;
    }

    //���� ���� (���߿� ����)
    private void IceStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceStartNotification;
    }

    //�ִϸ��̼� �ݿ�
    private void IcePlayersStateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayersStateSyncNotification;
    }

    //�÷��̾� ��� �̺�Ʈ
    private void IcePlayerDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDeathNotification;
    }

    //�� ���� �̺�Ʈ(���� ������ ��ȿ��, ������ ����)
    private void IceMapStateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMapStateSyncNotification;
    }

    //Ice �̴ϰ��� ����
    private void IceOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceOverNotification;
    }
}