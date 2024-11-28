using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    /* 소켓 매니저 가이드
     * 새로운 함수 만들 때 public void로 만들 것. (public 사유 : 리플렉션의 정상 작동)
     * 함수명 PayloadOneOfCase Enum과 맞출 것.
     * 인자는 GamePacket gamePacket.
     */

    #region Parse Messages
    //새로운 벡터 선언 (임시로)
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

    #region 인증 서버
    public void RegisterResponse(GamePacket packet)
    {
        var response = packet.RegisterResponse;
        UIManager.Get<UIRegister>().TrySetTask(response.Success);
        Debug.Log(response.Success);
        Debug.Log(response.FailCode);

        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LoginResponse(GamePacket packet)
    {
        var response = packet.LoginResponse;
        GameManager.Instance.myInfo.SetSessionId(response.SessionId);
        //UILogin.Instance.TrySetTask(response.Success);
        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }
    #endregion


    #region 로비 서버
    public void LobbyJoinResponse(GamePacket packet)
    {
        var response = packet.LobbyJoinResponse;
        GameManager.Instance.myInfo.userData = response.User;
        UIManager.Get<UILobby>().TrySetTask(response.Success);
        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LobbyLeaveResponse(GamePacket packet)
    {
        var response = packet.LobbyLeaveResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success);
        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    #endregion

    #region 룸 서버

    public void RoomListResponse(GamePacket packet)
    {
        var response = packet.RoomListResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success);
        if (response.Success) UIManager.Get<UILobby>().SetRoomList(response.Rooms);
        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void CreateRoomResponse(GamePacket packet)
    {
        var response = packet.CreateRoomResponse;
        UIManager.Get<UIMakeRoom>().TrySetTask(response.Success);
        UIManager.Show<UIRoom>(response.Room);
        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }


    public void JoinRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success);
        if (response.Success)
        {
            UIManager.Hide<UILobby>();
            UIManager.Show<UIRoom>(response.Room);
        }
        if ((int)response.FailCode != 0)
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void JoinRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomNotification;
        UIManager.Get<UIRoom>().AddRoomUser(response.User);
    }

    public void LeaveRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomResponse;
        bool isSuccess = UIManager.Get<UIRoom>().leaveRoomTcs.TrySetResult(response.Success);
        if (isSuccess)
        {
            Debug.Log("Leave Room Success");
        }
        else
        {
            // TODO:: FailCode에 맞는 알림바꾸기
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomNotification;
        UIManager.Get<UIRoom>().RemoveRoomUser(response.User.LoginId);
    }

    public void GamePrepareResponse(GamePacket packet)
    {
        var response = packet.GamePrepareResponse;
        bool isSuccess = UIManager.Get<UIRoom>().readyTcs.TrySetResult(response.Success);
        if (isSuccess)
        {
            UIManager.Get<UIRoom>().SetIsReady(response.IsReady);
        }
        else
        {
            // TODO:: FailCode에 맞는 알림바꾸기
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }

    public void GamePrepareNotification(GamePacket packet)
    {
        var response = packet.GamePrepareNotification;
        UIManager.Get<UIRoom>().SetUserReady(response.User.LoginId, response.IsReady, response.State);
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