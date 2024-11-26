using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RoomData   // Protocol.cs에서 만들어지는 클래스.
{
    public string name;
    public string number;
    public List<UserData> userDatas; // RoomData 안에 입장해있는 인원 정보도 있다고 가정함.
}
public class UserData
{
    public int userId;
    public string nickname;
}

public class UIRoom : UIBase
{
    [SerializeField] private bool isHost;
    public bool IsHost { get { return isHost; } }
    private bool isReady = false;
    [SerializeField] private List<RoomUserSlot> userSlots;

    [Header("Room Info")]
    [SerializeField] private TMP_Text roomNumber;
    [SerializeField] private TMP_Text roomName;
    private RoomData roomData;

    private UnityAction onUserReadyChanged;
    private bool[] isReadyUsers = new bool[4];

    [Header("Rule Setting")]
    [SerializeField] private TMP_Dropdown ddMaxTurn;
    private int[] turnOptions = { 10, 15, 20, 25, 30 };
    [Range(0, 4)] public int maxTurnValue = 0;
    private int maxTurn;

    [Header("Start Countdown")]
    [SerializeField] private TMP_Text count;
    [SerializeField] private GameObject invisibleWall;

    [Header("Button")]
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonStart;

    public TaskCompletionSource<bool> readyTcs;
    public TaskCompletionSource<bool> leaveRoomTcs;

    public override void Opened(object[] param)
    {
        roomData = (RoomData)param[0];

        Init();
        SetRoomInfo(roomData);
        if (isHost)
        {
            buttonStart.gameObject.SetActive(true);
            buttonReady.gameObject.SetActive(false);
            buttonStart.interactable = false;
        }
        else
        {
            buttonReady.gameObject.SetActive(true);
            buttonStart.gameObject.SetActive(false);
        }
    }

    public override async void HideDirect()
    {
        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

    private void Init()
    {
        SetIsHost();    // 호스트가 아닌 경우를 Test해보려면 이 줄을 주석처리하기.
        buttonReady.onClick.AddListener(OnReadyButtonClick);
        onUserReadyChanged += TryActiveStartButton;

        SetDropdown();
        SetUserReady(0);    // 방장은 자동 레디처리
    }
    
    // 방에 들어오면 자동으로 실행
    // 방 이름 정보 세팅
    // 방 유저정보 세팅
    public void SetRoomInfo(RoomData data)
    {
        roomData = data;
        roomNumber.text = (data.number != null) ? $"No. {data.number.ToString()}" : "";
        roomName.text = (data.name != null) ? data.name.ToString() : "";

        if (data.userDatas.Count > 0)
        {
            for (int i = 0; i < data.userDatas.Count; i++)
            {
                userSlots[i].SetRoomUser(data.userDatas[i].nickname, data.userDatas[i].userId);
            }
        }
        else
        {
            Debug.Log($"data.userDatas.Count : {data.userDatas.Count}");
        }


    }

    
    /// <summary>
    /// 방을 만들고 내가 첫 유저면 isHost true.
    /// 이건 서버가 할 일?
    /// </summary>
    public void SetIsHost()
    {
        //if (users[0] == null)
        //{
        //    users[0] = new UserInfo();
        //    isHost = true;
        //}
        //else
        //{
        //    for ( int i = 1; i < users.Length; ++i )
        //    {
        //        if (users[i] == null)
        //        {
        //            users[i] = new UserInfo();
        //            break;
        //        }
        //    }
        //}        
    }


    /// <summary>
    /// TODO:: S2C_GamePrepareNotification 를 받을 때 호출
    /// 
    /// </summary>
    /// <param name="userId"></param>
    public void SetUserReady(int userId)
    {
        if(isHost) isReady = true;



        if (userId >=0 && userId < isReadyUsers.Length)
        {
            isReadyUsers[userId] = true;

            onUserReadyChanged?.Invoke();
            UpdateReadyState(userId);
        }
    }
    private void UpdateReadyState(int userIndex)
    {
        userSlots[userIndex].Ready(isReady);
    }


    public void TryActiveStartButton()
    {
        if(IsReadyUsers())
        {
            buttonStart.interactable = true;
            // TODO::버튼이미지 변경 등 실행
            Debug.Log("모든 유저가 준비 완료. 시작버튼 활성화");
        }
        else
        {
            buttonStart.interactable = false;
            Debug.Log("아직 준비되지 않은 유저가 있습니다.");
            return;
        }
    }

    public bool IsReadyUsers()
    {
        // 모든 유저의 게임준비 노티파이를 받으면 true
        for(int i = 0; i < isReadyUsers.Length; i++)
        {
            if (!isReadyUsers[i])
            {
                return false;
            }
        }
        return true;
    }

    private async void GameStart()
    {
        // TODO:: 서버에 게임시작 패킷 보내기
        //GamePacket packet = new();
        //packet.게임시작 = new()
        //{
        // Maxturn = maxturn
        //};
        //SocketManager.Instance.OnSend(packet);



        await CountDownAsync(3);
        await UIManager.Show<UIFadeScreen>("FadeOut");
        invisibleWall.SetActive(false);


        // TODO:: 보드씬 로드
        SceneManager.LoadScene("BoardScene");
    }


    private async void OnReadyButtonClick()
    {
        buttonReady.interactable = false;

        if (isReady)
        {
            bool isSuccess = await CancelReadyAsync();
            if(isSuccess)
            {
                isReady = false;
                UpdateButtonUI("Ready", Color.grey, true);
            }
            else
            {
                Debug.Log("준비취소 실패");
                buttonReady.interactable = true;
            }
        }
        else
        {
            bool isSuccess = await ReadyAsync();
            if (isSuccess)
            {
                isReady = true;
                UpdateButtonUI("Cancel Ready", Color.green, true);
            }
            else
            {
                Debug.Log("준비완료 실패");
                buttonReady.interactable = true;
            }
        }
    }

    /// <summary>
    /// C2S_GamePrepareRequest 
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ReadyAsync()
    {
        readyTcs = new();
        // 서버에 준비 패킷 보내기
        //GamePacket packet = new();
        //packet.GamePrepareRequest = new()
        //{
        //    PlayerId = GameManager.Instance.GetPlayerId()
        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("서버로 준비 완료 패킷 전송 중...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("준비 완료 패킷 전송 성공");

        if (isSuccess)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
       
    private async Task<bool> CancelReadyAsync()
    {
        await Task.Delay(500);
        readyTcs = new();

        // 서버에 준비취소 패킷 보내기
        //GamePacket packet = new();
        //packet.게임준비취소 = new()
        //{
        //  PlayerId = GameManager.Instance.GetPlayerId()
        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("서버로 준비 취소 패킷 전송 중...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("준비 취소 패킷 전송 성공");
        if (isSuccess)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateButtonUI(string buttonText, Color color, bool interactable)
    {
        var textComponent = buttonReady.GetComponentInChildren<TMPro.TMP_Text>();
        if (textComponent != null )
        {
            textComponent.text = buttonText;
        }
        
        var imageComponent = buttonReady.GetComponentInChildren<Image>();
        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
        buttonReady.interactable = interactable;
    }

    

    #region GameSetting
    private void SetDropdown()
    {
        if (!isHost)
        {
            ddMaxTurn.interactable = false;
        }
        else
        {
            ddMaxTurn.interactable = true;

            maxTurn = turnOptions[maxTurnValue];
            ddMaxTurn.options.Clear();
            for (int i = 0; i < maxTurnValue + 1; ++i)
            {
                ddMaxTurn.options.Add(new TMP_Dropdown.OptionData(turnOptions[i].ToString()));
            }
            ddMaxTurn.onValueChanged.AddListener(OnDropdownEvent);
        }
    }
    private void OnDropdownEvent(int index)
    {
        maxTurn = turnOptions[index];
    }

    #endregion           

    #region Button
    public void ButtonBack()
    {
        BackLobby();
    }
    private async void BackLobby()
    {
        // TODO:: 방 나감 패킷 보내기


        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

    public void ButtonStart()
    {
        Debug.Log($"최대 턴 : {maxTurn}");
        GameStart();
    }
    #endregion

    #region 유저 입장


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
        foreach (RoomUserSlot user in userSlots)
        {
            if (user.playerId == playerId)
            {
                // 퇴장처리 실행
                user.LeaveRoomUser();
                break;
            }
        }
    }


    #endregion

    private async Task CountDownAsync(int countNum)
    {
        invisibleWall.SetActive(true);
        count.gameObject.SetActive(true);

        while (countNum > 0)
        {
            count.text = countNum--.ToString();
            await Task.Delay(1000);
        }
        count.gameObject.SetActive(false);
    }

    #region 소켓매니저에 작성될 메서드
    

    // 로비에서 C2S_JoinRoomRequest 요청을 보내고 S2C_JoinRoomResponse 받을 때 여기있는 메서드를 호출한다.   
    // 로비에서 만들어져 있는 방에 입장할 때 S2C_JoinRoomResponse 응답받고 호출한다.
    public void JoinRoomResponse(GamePacket gamePacket)
    {
        //var response = gamePacket.JoinRoomResponse;

        //RoomData roomData = response.RoomData;

        //if(response.Success)
        //{
        //    UIManager.Show<UIRoom>(response.RoomData);    
        //}
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


    /// <summary>
    /// S2C_GamePrepareResponse 
    /// </summary>
    /// <param name="packet"></param>
    public void GamePrepareResponse(GamePacket packet)
    {
        //var response = packet.GamePrepareResponse;
        //bool isSuccess = response.Success;
        //if (UIManager.Get<UIRoom>().readyTcs.TrySetResult(isSuccess))
        //{
        //    return;
        //}
        //else
        //{
        //    // TODO:: FailCode에 맞는 알림바꾸기
        //    Debug.LogError($"FailCode : {response.FailCode}");
        //}
    }

    /// <summary>
    /// S2C_GamePrepareNotification 
    /// </summary>
    /// <param name="packet"></param>
    public void GamePrepareNotification(GamePacket packet)
    {
        //var response = packet.GamePrepareNotification;
        //int readyUserId = response.UserId;

    }


    /* Leave
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


}