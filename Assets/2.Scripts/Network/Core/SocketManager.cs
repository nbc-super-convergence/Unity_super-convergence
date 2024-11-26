using UnityEngine;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    /* ���� �Ŵ��� ���̵�
     * ���ο� �Լ� ���� �� public void�� ���� ��. (public ���� : ���÷����� ���� �۵�)
     * �Լ��� PayloadOneOfCase Enum�� ���� ��.
     * ���ڴ� GamePacket gamePacket.
     */

    #region Receive Packets
    //���� ���� �˸� Receive.
    public void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;

        //���� �α��� �ܰ�� �̵�.
        GameManager.Instance.SetPlayerId(response.PlayerId); 

        //init Minigame
        MiniGameManager.Instance.SetMiniGame<GameIceSlider>();

        //Spawn MiniPlayer
        MiniGameManager.Instance.GetMiniPlayer(response.PlayerId)
            .ReceivePlayerSpawn(ConvertVector3(response.Position), response.Rotation);
    }

    //�ٸ� �÷��̾� ������ Receive.
    public void IcePlayerMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveNotification;
        foreach (var p in response.Players)
        {
            Debug.LogWarning($"ReceiveClientMove : {p.PlayerId}");
            MiniGameManager.Instance.GetMiniPlayer(p.PlayerId)
                .ReceiveOtherMove(ConvertVector3(p.Position), ConvertVector3(p.Force), p.Rotation, p.State);
        }
    }
    #endregion

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