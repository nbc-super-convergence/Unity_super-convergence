using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS4014
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
    public static Vector ToVector(Vector3 other)
    {
        return new Vector
        {
            X = other.x,
            Y = other.y,
            Z = other.z,
        };
    }

    public static Vector3 ToVector3(Vector other)
    {
        return new Vector3()
        {
            x = other.X,
            y = other.Y,
            z = other.Z
        };
    }
    #endregion

    #region 공통
    public void CloseSocketNotification(GamePacket packet)
    {
        var response = packet.CloseSocketNotification;

        if(SceneManager.GetActiveScene().buildIndex == 2)
            BoardManager.Instance.ExitPlayer(response.SessionId);

        GameManager.Instance.DeleteSessionId(response.SessionId);
    }
    #endregion

    #region 인증 서버
    public void RegisterResponse(GamePacket packet)
    {
        var response = packet.RegisterResponse;
        UIManager.Get<UIRegister>().TrySetTask(response.Success);
        if ((int)response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LoginResponse(GamePacket packet)
    {
        var response = packet.LoginResponse;
        GameManager.Instance.myInfo.SetSessionId(response.SessionId);
        UIManager.Get<UILogin>().TrySetTask(response.Success);        
        if ((int)response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }
    #endregion


    #region 로비 서버
    public void LobbyJoinResponse(GamePacket packet)
    {
        var response = packet.LobbyJoinResponse;
        GameManager.Instance.myInfo.SetNickname(response.User.Nickname);
        UIManager.Get<UILobby>().TrySetTask(response.Success, UILobby.eTcs.Lobby);

        if (response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LobbyLeaveResponse(GamePacket packet)
    {
        var response = packet.LobbyLeaveResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success, UILobby.eTcs.Lobby);
        if ((int)response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void LobbyUserListResponse(GamePacket packet)
    {
        var response = packet.LobbyUserListResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success, UILobby.eTcs.User);
        if (response.Success) UIManager.Get<UILobby>().SetUserList(response.UserList);
        if (response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    #endregion

    #region 룸 서버
    public void RoomListResponse(GamePacket packet)
    {
        var response = packet.RoomListResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success, UILobby.eTcs.Room);
        if (response.Success) UIManager.Get<UILobby>().SetRoomList(response.Rooms);
        if ((int)response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void CreateRoomResponse(GamePacket packet)
    {
        var response = packet.CreateRoomResponse;
        UIManager.Get<UIMakeRoom>().TrySetTask(response.Success);
        UIManager.Show<UIRoom>(response.Room);
        if (response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }

    public void JoinRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomResponse;
        UIManager.Get<UILobby>().TrySetTask(response.Success, UILobby.eTcs.Lobby);
        if (response.Success)
        {
            UIManager.Hide<UILobby>();
            UIManager.Show<UIRoom>(response.Room);
        }
        else 
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void JoinRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomNotification;
        UIManager.Get<UIRoom>()?.OnRoomMemberChange(response.Room, true);
    }

    public void LeaveRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomResponse;
        UIManager.Get<UIRoom>().TrySetTask(response.Success);        

        if (response.FailCode != 0)
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }

    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomNotification;
        UIManager.Get<UIRoom>()?.OnRoomMemberChange(response.Room, false);
    }

    public void GamePrepareResponse(GamePacket packet)
    {
        var response = packet.GamePrepareResponse;
        UIManager.Get<UIRoom>().TrySetTask(response.Success);
        if (response.Success)
        {
            UIManager.Get<UIRoom>().SetUserReady(response.IsReady);
        }
        else
        {
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }

    public void GamePrepareNotification(GamePacket packet)
    {
        var response = packet.GamePrepareNotification;

        UIManager.Get<UIRoom>()?.SetUserReady(response.IsReady, response.User.SessionId, response.State);
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
            UIManager.Show<UIError>(response.FailCode);
            Debug.LogError($"FailCode : {response.FailCode}");
        }
    }
    
    public void RoomKickRequest(GamePacket packet)
    {

    }

    public void RoomKickResponse(GamePacket packet)
    {
        var response = packet.RoomKickResponse;

        if (response.Success)
        {
            RoomData roomData = response.Room;
            UIManager.Get<UIRoom>().OnKickEvent(false, roomData);
        }
        else
        {
            UIManager.Show<UIError>(response.FailCode);
        }
    }

    public void RoomKickNotification(GamePacket packet)
    {
        var response = packet.RoomKickNotification;

        RoomData roomData = response.Room;
        bool isKicked = GameManager.Instance.myInfo.SessionId == response.TargetSessionId;
        UIManager.Get<UIRoom>().OnKickEvent(isKicked, roomData);
    }
    #endregion
}