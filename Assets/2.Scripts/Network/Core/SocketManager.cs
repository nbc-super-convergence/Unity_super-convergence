using UnityEngine;
using static S2C_IcePlayerMoveNotification.Types;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    /* ���� �Ŵ��� ���̵�
     * ���ο� �Լ� ���� �� public void�� ���� ��. (public ���� : ���÷����� ���� �۵�)
     * �Լ��� PayloadOneOfCase Enum�� ���� ��.
     * ���ڴ� GamePacket gamePacket.
     */
    
    
    //���� ���� �˸� Receive.
    public void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;

        IceBoardPlayerManager.Instance.SpawnPosition(response);
    }

    //�ٸ� �÷��̾� ������ Receive.
    public void IcePlayerMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveNotification;
        int playerSize = response.Players.Count;
        PlayerData data = response.Players[0];

        IceBoardPlayerManager.Instance.ReceivePosition(response);

        //Debug.Log(response);
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

    public static Vector3 ConvertVector3(Vector other)
    {
        return new Vector3()
        {
            x = other.X,
            y = other.Y,
            z = other.Z
        };
    }
}

