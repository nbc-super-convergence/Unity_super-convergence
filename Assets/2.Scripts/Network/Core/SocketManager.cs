using UnityEngine;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    /* ���� �Ŵ��� ���̵�
     * ���ο� �Լ� ���� �� public void�� ���� ��. (public ���� : ���÷����� ���� �۵�)
     * �Լ��� PayloadOneOfCase Enum�� ���� ��.
     * ���ڴ� GamePacket gamePacket.
     */

    #region Parse Messages
    //���ο� ���� ���� (�ӽ÷�)
    public static Vector ConvertVector(Vector3 other)
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
    #endregion
}