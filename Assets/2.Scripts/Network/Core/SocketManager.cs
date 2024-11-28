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

    public void LoginResponse(GamePacket packet)
    {
        var response = packet.LoginResponse;
        GameManager.Instance.myInfo.SetSessionId(response.SessionId);
    }

    public void LobbyJoinResponse(GamePacket packet)
    {
        var response = packet.LobbyJoinResponse;
        GameManager.Instance.myInfo.userData = response.UserData;
    }

    #region ����
    public void JoinRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomResponse;

        if (response.Success)
        {
            UIManager.Show<UIRoom>(response.Room);
        }
    }

    public void JoinRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomNotification;
        UIManager.Get<UIRoom>().AddRoomUser(response.UserData);
    }

    public void LeaveRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomResponse;
        if (UIManager.Get<UIRoom>().leaveRoomTcs.TrySetResult(response.Success))
        {
            Debug.Log("Leave Room Success");
        }
        else
        {
            // TODO:: FailCode�� �´� �˸��ٲٱ�
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomNotification;
        UIManager.Get<UIRoom>().RemoveRoomUser(response.SessionId);
    }

    public void GamePrepareResponse(GamePacket packet)
    {
        var response = packet.GamePrepareResponse;
        if (UIManager.Get<UIRoom>().readyTcs.TrySetResult(response.Success))
        {
            UIManager.Get<UIRoom>().SetIsReady(response.IsReady);
        }
        else
        {
            // TODO:: FailCode�� �´� �˸��ٲٱ�
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }

    public void GamePrepareNotification(GamePacket packet)
    {
        var response = packet.GamePrepareNotification;
        UIManager.Get<UIRoom>().SetUserReady(response.SessionId, response.IsReady, response.State);
    }

    public void GameStartNotification(GamePacket packet)
    {
        var response = packet.GameStartNotification;
        if (response.Success)
        {
            UIManager.Get<UIRoom>().GameStart();
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }
    #endregion

}