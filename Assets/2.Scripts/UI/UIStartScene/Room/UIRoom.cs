using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoom : UIBase
{
    private bool isReady = false;

    [SerializeField] private List<RoomUserSlot> userSlots;
    private List<UserData> users = new();

    [Header("Room Info")]
    [SerializeField] private TMP_Text roomNumber;
    [SerializeField] private TMP_Text roomName;
    private RoomData roomData;
    private RoomStateType state;
    [SerializeField] private bool isHost;
    public bool IsHost { get { return isHost; } }
    private string currentOwnerId;

    [Header("Rule Setting")]
    [SerializeField] private TMP_Dropdown ddMaxTurn;
    private int[] turnOptions = { 10, 15, 20, 25, 30 };
    [Range(0, 4)] public int maxTurnValue = 0;
    private int maxTurn = 10;

    [Header("Start Countdown")]
    [SerializeField] private TMP_Text count;
    [SerializeField] private GameObject invisibleWall;

    [Header("Button")]
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonStart;

    private TaskCompletionSource<bool> sourceTcs;

    #region 대기방 관리
    public override void Opened(object[] param)
    {
        roomData = (RoomData)param[0];
        SetRoomInfo(roomData);
    }

    public override async void HideDirect()
    {
        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

    public void TrySetTask(bool isSuccess)
    {
        bool boolll = sourceTcs.TrySetResult(isSuccess);
        Debug.Log(boolll ? "성공" : "실패");
    }

    private void Init()
    {
        GameManager.Instance.SessionDic.Clear();
        int num = 0;
        foreach (var user in roomData.Users)
        {
            // TODO:: 없는 유저는 -1로 남게... 
            GameManager.Instance.SessionDic.Add(user.SessionId, new UserInfo(user.SessionId, user.Nickname, num, num));
            num += 1;
        }
        SetHost();
        SetDropdown();       
        // TODO:: 해쉬같은거 써서 깔끔하게.
        if(isHost)
        {
            foreach ( var item in userSlots)
            {
                if(item.sessionId == roomData.OwnerId)
                {
                    item.CheckReadyState(true, currentOwnerId);
                }
            }            
        }

        if (isHost)
        {
            if (this.state == RoomStateType.Prepare ? true : false)
            {
                buttonStart.interactable = true;
                Debug.Log("모든 유저가 준비 완료. 시작버튼 활성화");
            }
            else
            {
                buttonStart.interactable = false;
                Debug.Log("아직 준비되지 않은 유저가 있습니다.");
            }
        }
    }
    
    private void SetHost()
    {
        currentOwnerId = roomData.OwnerId;

        isHost = (roomData.OwnerId == GameManager.Instance.myInfo.SessionId) ? true : false;

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

    /// <summary>
    /// 유저 변동이 있을 때 서버로부터 RoomData 받아서 모든 정보를 갱신함.
    /// </summary>
    /// <param name="data"></param>
    public void SetRoomInfo(RoomData data)
    {
        ClearUserSlot();

        roomData = data;

        Init();

        //roomNumber.text = (data.RoomId != null) ? $"No. {data.RoomId}" : "";
        roomName.text = (data.RoomName != null) ? data.RoomName : "";

        for (int i = 0; i < data.Users.Count; i++)
        {
            if (data.Users[i].SessionId == GameManager.Instance.myInfo.SessionId)
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

        ReadyUsersSync(data);
    }

    public void AddRoomUser(UserData userData)
    {
        users.Add(userData);
        for(int i = 0; i < userSlots.Count; ++i)
        {
            UserData userInfo = users.Count > i ? users[i] : null;
            if(userInfo != null)
            {
                userSlots[i].SetRoomUser(userInfo, i);
            }
            else
            {
                userSlots[i].EmptyRoomUser();
            }
        }
    }

    // LeaveRoomNotification
    public void RemoveRoomUser(string sessionId)
    {
        foreach (RoomUserSlot user in userSlots)
        {
            if (user.sessionId == sessionId)
            {
                user.EmptyRoomUser();
                break;
            }
        }

        users.RemoveAll(obj => obj.SessionId == sessionId);
        if (sessionId == currentOwnerId)
        {
            roomData.OwnerId = users[0].SessionId;
            SetHost();
        }
        for (int i = 0; i < userSlots.Count; ++i)
        {
            UserData userInfo = users.Count > i ? users[i] : null;
            userSlots[i].SetRoomUser(userInfo, i);
        }
    }

    private void ClearUserSlot()
    {
        foreach(RoomUserSlot user in userSlots)
        {
            user.EmptyRoomUser();
        }
        users.Clear();
    }

    private void ReadyUsersSync(RoomData roomData)
    {        
        foreach (RoomUserSlot user in userSlots)
        {           
            if(roomData.ReadyUsers.Contains(user.sessionId) || roomData.OwnerId == user.sessionId)
            {
                user.CheckReadyState(true, roomData.OwnerId);
            }
        }
    }

    #endregion

    #region 준비
    private async void OnClickReadyButton()
    {
        buttonReady.interactable = false;

        // TODO:: 코드압축 가능. but 가독성 떨어질것 같음.
        if (isReady)
        {
            // 준비취소하기
            bool isSuccess = await ReadyAsync(false);
            if (isSuccess)
            {
                //isReady = false; 서버에서 받음
                UpdateButtonUI("준비하기", Color.white);
            }
            else
            {
                Debug.Log("준비취소 실패");
            }
        }
        else
        {
            // 준비하기
            bool isSuccess = await ReadyAsync(true);
            if (isSuccess)
            {
                //isReady = true;
                UpdateButtonUI("준비 취소", Color.grey);
            }
            else
            {
                Debug.Log("준비완료 실패");                
            }
        }
        buttonReady.interactable = true;
    }
    
    /// <summary>
    /// C2S_GamePrepareRequest 
    /// S2C_GamePrepareResponse
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ReadyAsync(bool isReady)
    {
        GamePacket packet = new();
        packet.GamePrepareRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            IsReady = isReady
        };    
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await sourceTcs.Task;

        return isSuccess ? true : false;
    }

    // GamePrepareResponse
    public void SetIsReady(bool isReady)
    {
        this.isReady = isReady;
        SetUserReady(GameManager.Instance.myInfo.SessionId, this.isReady, this.state);
    }

    private void UpdateButtonUI(string buttonText, Color color)
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
    }

    /// <summary>
    /// S2C_GamePrepareNotification 를 받을 때 호출
    /// 준비와 준비취소 모두 대응.
    /// 준비한 유저의 sessionId. sessionId의 준비상태 isReady. 현재 방의 상태 state.
    /// </summary>
    public void SetUserReady(string sessionId, bool isReady, RoomStateType state)
    {
        bool isError = true;
        // TODO::딕셔너리나 Hash를 이용.
        foreach(RoomUserSlot user in userSlots)
        {
            if(user.sessionId == sessionId)
            {
                user.CheckReadyState(isReady, currentOwnerId);                
                isError = false;
                break;
            }            
        }
        if (isError) Debug.LogError($"userSlots에 {sessionId}가 없습니다.");

        this.state = state;
        Debug.Log($"현재 대기방 상태: {this.state}");    
        if(isHost)
        {
            if (this.state == RoomStateType.Prepare ? true : false)
            {
                buttonStart.interactable = true;
                Debug.Log("모든 유저가 준비 완료. 시작버튼 활성화");
            }
            else
            {
                buttonStart.interactable = false;
                Debug.Log("아직 준비되지 않은 유저가 있습니다.");
            }
        }
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
            SessionId = GameManager.Instance.myInfo.SessionId,
            Turn = maxTurn,
        };
        SocketManager.Instance.OnSend(packet);
    }

    /// <summary>
    /// S2C_GameStartNotification
    /// GameStartNotification은 방장 포함 모든 유저가 받음.
    /// </summary>
    public async void GameStart()
    {
        await CountDownAsync(3);
        //await UIManager.Show<UIFadeScreen>("FadeOut");
        FadeScreen.Instance.FadeOut(Capsule, 1.5f);        
        void Capsule()
        {
            invisibleWall.SetActive(false);
            GameManager.isGameStart = true;
        }
    }
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
        sourceTcs = new();

        GamePacket packet = new();
        packet.LeaveRoomRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId            
        };
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await sourceTcs.Task;

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


}