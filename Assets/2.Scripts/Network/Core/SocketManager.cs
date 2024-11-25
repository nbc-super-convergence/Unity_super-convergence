using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GamePacket;
using UnityEngine.UIElements;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    //TODO: ���ο� �Լ� ���� �� private void�� ���� ��.
    //TODO: �Լ��� �̸��� �ݵ�� PayloadOneOfCase Enum�� ���� ��.
    //TODO: ���ڴ� GamePacket gamePacket.

    //���� ���� Send.
    public void IceJoinRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IceJoinRequest;
    }

    //���� ���� �˸� Receive.
    public void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;
        int CurrentId = response.PlayerType;
        Vector3 myStartPos = new Vector3(response.Position.X, response.Position.Y, response.Position.Z);
        float myRotY = response.Rotation;
    }

    //���� �÷��̾� ������ Send.
    public void IcePlayerMoveRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveRequest;
    }

    //�ٸ� �÷��̾� ������ Receive.
    public void IcePlayerMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveNotification;
    }

    //���ο� ���� ���� (�ӽ÷�)
    public static Vector CreateVector(Vector3 other)
    {
        return new Vector
        {
            X = other.x,
            Y = other.y,
            Z = other.z,
        };
    }
}