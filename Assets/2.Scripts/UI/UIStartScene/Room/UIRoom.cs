using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIRoom : UIBase
{
    [SerializeField] private bool isHost;
    public bool IsHost { get { return isHost; } }
    private bool isReady = false;
    private int readyCount = 0;

    [SerializeField] private List<RoomUserSlot> userSlots;
    private List<UserData> users = new();

    [Header("Room Info")]
    [SerializeField] private TMP_Text roomNumber;
    [SerializeField] private TMP_Text roomName;
    private RoomData roomData;
    private RoomStateType state;

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

    #region 대기방 관리
    public override void Opened(object[] param)
    {
        roomData = (RoomData)param[0];

        Init();
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
        SetRoomInfo(roomData);
    }

    public override async void HideDirect()
    {
        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

    private void Init()
    {
        isHost = (roomData.OwnerId == GameManager.Instance.myInfo.sessionId) ? true : false;

        SetDropdown();
        //if (isHost) ButtonReady();    // 방장은 자동 레디처리
    }
    
    public void SetRoomInfo(RoomData data)
    {
        roomData = data;
        roomNumber.text = (data.RoomId != null) ? $"No. {data.RoomId}" : "";
        roomName.text = (data.RoomName != null) ? data.RoomName : "";

        for (int i = 0; i < data.Users.Count; i++)
        {
            if (data.Users[i].LoginId == GameManager.Instance.myInfo.userData.LoginId)  // sessionId
            {
                AddRoomUser(GameManager.Instance.myInfo.ToUserData());
            }
            else
            {
                AddRoomUser(data.Users[i]);
            }
        }
        if (data.Users.Count == 0)
        {
            AddRoomUser(GameManager.Instance.myInfo.ToUserData());
        }
    }

    public void AddRoomUser(UserData userData)
    {
        users.Add(userData);
        for(int i = 0; i < userSlots.Count; ++i)
        {
            UserData userInfo = users.Count > i ? users[i] : null;
            if(userInfo != null)
            {
                userSlots[i].SetRoomUser(userInfo);
            }
            else
            {
                userSlots[i].EmptyRoomUser();
            }
        }
    }

    public void RemoveRoomUser(string userId)// sessionI
    {
        // TODO::user의 처리에 맞게 바꾸기
        foreach (RoomUserSlot user in userSlots)
        {
            if (user.loginId == userId)// sessionI
            {
                user.EmptyRoomUser();
                break;
            }
        }

        users.RemoveAll(obj => obj.LoginId == userId);// sessionI
        for (int i = 0; i < userSlots.Count; ++i)
        {
            UserData userInfo = users.Count > i ? users[i] : null;
            userSlots[i].SetRoomUser(userInfo);
        }
    }
    #endregion

    #region 준비
    private async void OnClickReadyButton()
    {
        buttonReady.interactable = false;

        if (isReady)
        {
            bool isSuccess = await CancelReadyAsync();
            if (isSuccess)
            {
                //isReady = false; 서버에서 받음
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
                //isReady = true;
                UpdateButtonUI("Cancel Ready", Color.green, true);
            }
            else
            {
                Debug.Log("준비완료 실패");
                buttonReady.interactable = true;
            }
        }
    }

    public void SetIsReady(bool isReady)
    {
        this.isReady = isReady;
    }

    /// <summary>
    /// C2S_GamePrepareRequest 
    /// S2C_GamePrepareResponse
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ReadyAsync()
    {
        readyTcs = new();

        GamePacket packet = new();
        packet.GamePrepareRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.sessionId,
            IsReady = true
        };    
        SocketManager.Instance.OnSend(packet);

        Debug.Log("서버로 준비 완료 패킷 전송 중...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("준비 완료 패킷 전송 성공");

        return isSuccess ? true : false;
    }

    private async Task<bool> CancelReadyAsync()
    {
        readyTcs = new();

        // 서버에 준비취소 패킷 보내기
        GamePacket packet = new();
        packet.GamePrepareRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.sessionId,
            IsReady = false
        };
        SocketManager.Instance.OnSend(packet);

        Debug.Log("서버로 준비 취소 패킷 전송 중...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("준비 취소 패킷 전송 성공");

        return isSuccess ? true : false;
    }

    private void UpdateButtonUI(string buttonText, Color color, bool interactable)
    {
        var textComponent = buttonReady.GetComponentInChildren<TMPro.TMP_Text>();
        if (textComponent != null)
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

    /// <summary>
    /// S2C_GamePrepareNotification 를 받을 때 호출
    /// 준비와 준비취소 모두 대응.
    /// </summary>
    public void SetUserReady(string sessionId, bool isReady, RoomStateType state)
    {
        bool isError = true;
        foreach(RoomUserSlot user in userSlots)
        {
            if(user.loginId == sessionId)
            {
                user.CheckReadyState(isReady, isHost);
                readyCount = isReady ? readyCount++ : readyCount--;
                this.state = (RoomStateType)state;
                Debug.Log($"현재 대기방 상태: {this.state}");

                if (readyCount == 4 ? true : false)
                {
                    buttonStart.interactable = true;
                    // TODO::버튼이미지 변경 등 실행
                    Debug.Log("모든 유저가 준비 완료. 시작버튼 활성화");
                }
                else
                {
                    buttonStart.interactable = false;
                    Debug.Log("아직 준비되지 않은 유저가 있습니다.");
                }
                isError = false;
                break;
            }            
        }
        if (isError) Debug.LogError($"userSlots에 {sessionId}가 없습니다.");
    }
    #endregion

    #region 시작
    /// <summary>
    /// C2S_GameStartRequest
    /// 방장은 GameStart 버튼을 눌러 서버에 게임시작 요청을 보낸다. 보드씬이 로드되는건 S2C_GameStartNotification 알림에서 실행할거다.
    /// </summary>
    private void OnClickGameStartButton()
    {
        GamePacket packet = new();
        packet.GameStartRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.sessionId
        };
        SocketManager.Instance.OnSend(packet);
    }

    /// <summary>
    /// S2C_GameStartNotification
    /// </summary>
    public async void GameStart()
    {
        await CountDownAsync(3);
        await UIManager.Show<UIFadeScreen>("FadeOut");
        invisibleWall.SetActive(false);

        // TODO:: 보드씬 로드
        SceneManager.LoadScene("BoardScene");
    }
    #endregion

    #region GameSetting
    // 최대 턴 변경을 적용하려면 알림 패킷이 필요함.
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
        LeaveRoom();
    }
    
    public void ButtonStart()
    {
        Debug.Log($"최대 턴 : {maxTurn}");
        OnClickGameStartButton();
    }

    public void ButtonReady()
    {
        OnClickReadyButton();
    }
    #endregion

    #region 유저 퇴장       

    // S2C_LeaveRoomResponse  
    // C2S_LeaveRoomRequest에 대한 리스폰스    
    // 유저가 퇴장버튼UI를 누르면
    public async void LeaveRoom()
    {
        leaveRoomTcs = new();

        GamePacket packet = new();
        packet.LeaveRoomRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.sessionId
        };
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await leaveRoomTcs.Task;

        if (isSuccess)
        {
            UIManager.Hide<UIRoom>();
            await UIManager.Show<UILobby>();
        }
        else
        {
            Debug.LogError("서버와 통신실패. 방나가기 실패");
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

}