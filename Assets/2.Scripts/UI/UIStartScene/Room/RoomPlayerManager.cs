using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class RoomPlayerManager : Singleton<RoomPlayerManager>
{
    [SerializeField]private RoomUser[] users = new RoomUser[4]; // TODO:: 바꿀수도 있음.

    [SerializeField] private StringBuilder sbUser = new StringBuilder();

    public TaskCompletionSource<bool> leaveRoomTcs;


    #region 테스트코드
    string[] strings0 = { "손효재", "정승연", "탁혁재", "박인수" };
    string[] strings1 = { "박인수", "손효재", "정승연", "탁혁재" };
    string[] strings2 = { "손효재", "정승연", "", "" };

    int[] ints0 = { 6521, 4789, 35478, 1123 };
    int[] ints1 = { 1123, 6521, 4789, 35478 };
    int[] ints2 = { 6521, 4789, 0, 0 };


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ReceiveServer(strings0, ints0);
        if (Input.GetKeyDown(KeyCode.S)) ReceiveServer(strings1, ints1);
        if (Input.GetKeyDown(KeyCode.D)) ReceiveServer(strings2, ints2);
    }
    #endregion

    

    /// <summary>
    /// 서버로부터 리스폰스or 노티스 받을 때 실행될 메서드. 패킷의 정보를 저장, 갱신한다.
    /// UserData배열을 받았을 때 실행
    /// 새로 방에 들어온 유저가 기존에 들어와있는 유저들의 정보를 표시
    /// </summary>   
    public void ReceiveServer(string[] nickNames, int[] userId)
    {
        for (int i = 0; i < sbUser.Length; i++)
        {            
            sbUser.Clear();
            sbUser.Append(nickNames[i]);
            users[i].SetRoomUser(sbUser[i].ToString(), userId[i]);
            users[i].SetImage();
        }

        Debug.Log($"현재 유저: {string.Join(" ", nickNames)}");
    }


    #region 유저 입장
    // S2C_JoinRoomNotification 을 받을 경우 실행
    // UserData 한개만 받았을때 실행
    public void JoinRoomNotification(UserData data)
    {
        foreach (RoomUser user in users)
        {
            if(user == null)
            {
                user.SetRoomUser(data.nickname, data.userId);
                break;
            }
        }
    }

    #endregion


    #region 유저 퇴장
    
    // C2S_LeaveRoomRequest
    // userId가 퇴장한다는 신호를 보낸다.
    public void LeaveRoomRequest(int playerId)
    {
        //GamePacket packet = new();
        //packet.C2S_LeaveRoomRequest = new()
        //{
        //    PlayerId = playerId
        //};
        //SocketManager.Instance.OnSend(packet);
    }

    // S2C_LeaveRoomResponse  
    // C2S_LeaveRoomRequest에 대한 리스폰스    
    // 유저가 퇴장버튼UI를 누르면
    public async void LeaveRoom()
    {
        //leaveRoomTcs = new();
        //LeaveRoomRequest(GameManager.Instance.GetPlayerId());

        //bool isSuccess = await leaveRoomTcs.Task;

        //if (isSuccess)
        //{
        //    UIManager.Hide<UIRoom>();
        //    await UIManager.Show<UILobby>();
        //}
        //else
        //{
        //    Debug.LogError("서버와 통신실패. 방나가기 실패");
        //}
    }

    // S2C_LeaveRoomNotification  
    // 다른유저의 퇴장알림을 받으면...
    public void LeaveRoomUserNotification(int playerId)
    {
        // TODO::user의 처리에 맞게 바꾸기
        foreach(RoomUser user in users)
        {
            if(user.userId == playerId)
            {
                // 퇴장처리 실행
                user.LeaveRoomUser();
                break;
            }
        }
    }


    #endregion

    /*
    #region 소켓매니저에 작성될 메서드
    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomNotification;

        JoinRoomNotification(response.UserData);
    }

    public void LeaveRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomResponse;

        if(response.Success)
        {
            RoomPlayerManager.Instance.leaveRoomTcs.TrySetResult(true);
        }
        else
        {
            // TODO:: FailCode에 맞는 알림바꾸기
            Debug.LogError($"FailCode : {response.FailCode}");
            RoomPlayerManager.Instance.leaveRoomTcs.TrySetResult(false);
        }
    }

    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomNotification;

        RoomPlayerManager.Instance.LeaveRoomUserNotification(response.UserId);
    }


    #endregion
    */

    public void ReadyThem(int index, bool isReady)
    {
        users[index].Ready(isReady);
    }
}


public class UserData
{
    public int userId;
    public string nickname;
}