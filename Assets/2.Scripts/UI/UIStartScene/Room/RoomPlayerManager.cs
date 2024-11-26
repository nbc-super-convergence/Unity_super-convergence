using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class RoomPlayerManager : Singleton<RoomPlayerManager>
{
    [SerializeField]private List<RoomUserSlot> userSlots = new(); 

    [SerializeField] private StringBuilder sbUser = new StringBuilder();

    public TaskCompletionSource<bool> leaveRoomTcs;


    #region 테스트코드
    //string[] strings0 = { "손효재", "정승연", "탁혁재", "박인수" };
    //string[] strings1 = { "박인수", "손효재", "정승연", "탁혁재" };
    //string[] strings2 = { "손효재", "정승연", "", "" };

    //int[] ints0 = { 6521, 4789, 35478, 1123 };
    //int[] ints1 = { 1123, 6521, 4789, 35478 };
    //int[] ints2 = { 6521, 4789, 0, 0 };


    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A)) ReceiveServer(strings0, ints0);
    //    if (Input.GetKeyDown(KeyCode.S)) ReceiveServer(strings1, ints1);
    //    if (Input.GetKeyDown(KeyCode.D)) ReceiveServer(strings2, ints2);
    //}
    #endregion

    

    ///// <summary>
    ///// 서버로부터 리스폰스or 노티스 받을 때 실행될 메서드. 패킷의 정보를 저장, 갱신한다.
    ///// UserData배열을 받았을 때 실행
    ///// 새로 방에 들어온 유저가 기존에 들어와있는 유저들의 정보를 표시
    ///// </summary>   
    //public void ReceiveServer(string[] nickNames, int[] userId)
    //{
    //    for (int i = 0; i < sbUser.Length; i++)
    //    {            
    //        sbUser.Clear();
    //        sbUser.Append(nickNames[i]);
    //        userSlots[i].SetRoomUser(sbUser[i].ToString(), userId[i]);
    //        userSlots[i].SetImage();
    //    }

    //    Debug.Log($"현재 유저: {string.Join(" ", nickNames)}");
    //}


    #region 유저 입장

    // 새로입장한 유저를 위해 대기방의 유저데이터 모두 받기
    public void SetRoomUserSlot(List<UserData> datas)
    {
        if(userSlots.Count <= 0) 
        {
            Debug.Log($"");
            return; 
        }

        for (int i = 0; i < datas.Count; i++)
        {
            userSlots[i].SetRoomUser(datas[i].nickname, datas[i].userId);
        }
    }

    // 로비에서 C2S_JoinRoomRequest 요청을 보내고 S2C_JoinRoomResponse 받을 때 여기있는 메서드를 호출한다.   
    // 로비에서 만들어져 있는 방에 입장할 때 S2C_JoinRoomResponse 응답받고 호출한다.
    public void JoinRoomResponse(GamePacket gamePacket)
    {
        //var response = gamePacket.JoinRoomResponse;

        //bool success = response.Success;
        //RoomData roomData = response.RoomData;

        //if(success)
        //{
        //    UIManager.Get<UIRoom>().SetRoomInfo(roomData);
        //    RoomPlayerManager.Instance.SetRoomUserSlot(roomData.userDatas);
        //}
    }    

    
    // S2C_JoinRoomNotification 을 받을 경우 실행
    public void SetRoomUserSlot(UserData data)
    {
        foreach (RoomUserSlot user in userSlots)    //TODO:: 바꿀 필요가 있어보임?
        {
            if (user == null)
            {
                user.SetRoomUser(data.nickname, data.userId);
                break;
            }
        }
    }
    /// <summary>
    /// S2C_JoinRoomNotification 
    /// SocketManager. 새로운 유저가 대기방에 Join했을 때 대기방 기존 유저들에게 알림.
    /// </summary>
    /// <param name="gamePacket"></param>
    public void JoinRoomNotification(GamePacket gamePacket)
    {
        //var response = gamePacket.JoinRoomNotification;
        //RoomPlayerManager.Instance.SetRoomUserSlot(response.UserData);
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
        foreach(RoomUserSlot user in userSlots)
        {
            if(user.playerId == playerId)
            {
                // 퇴장처리 실행
                user.LeaveRoomUser();
                break;
            }
        }
    }


    #endregion

    #region 소켓매니저에 작성될 메서드
    /*
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

    */

    #endregion

    public void ReadyThem(int index, bool isReady)
    {
        userSlots[index].Ready(isReady);
    }
}

