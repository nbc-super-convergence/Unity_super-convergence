using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RoomData   // Protocol.cs���� ��������� Ŭ����.
{
    public string id;
    public string ownerId;
    public string name;
    public string lobbyId;
    RoomStateType state;
    public List<UserData> users; 
    public int maxUsers;
    public List<string> readyUsers;
}
public class UserData
{
    public int loginId;
    public string nickname;
    public UserData(int loginId, string nickname)
    {
        this.loginId = loginId;
        this.nickname = nickname;
    }
}
enum RoomStateType
{
    WAIT = 0,
    PREPARE = 1,
    INGAME = 2
}

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

    #region ���� ����
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
        //isHost = (roomData.ownerId == GameManager.Instance.SessionId) ? true : false;

        SetDropdown();
        if (isHost) ButtonReady();    // ������ �ڵ� ����ó��
    }
    
    public void SetRoomInfo(RoomData data)
    {
        roomData = data;
        roomNumber.text = (data.id != null) ? $"No. {data.id.ToString()}" : "";
        roomName.text = (data.name != null) ? data.name.ToString() : "";

        //for (int i = 0; i < data.userDatas.Count; i++)
        //{
        //    if (data.userDatas[i].userId == GameManager.Instance.PlayerId)
        //    {
        //        AddRoomUser(data.userDatas[i]);    // TODO::Ŭ�� �����ص� �������� �ֱ�
        //    }
        //    else
        //    {
        //        AddRoomUser(data.userDatas[i]);
        //    }
        //}
        //if(data.userDatas.Count == 0) 
        //{
        //    AddRoomUser(new UserData(GameManager.Instance.PlayerId, GameManager.Instance.nickName);     // TODO::Ŭ�� �����ص� �������� �ֱ� (������ RoomData�� ��������Ŵ�.)
        //}
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

    public void RemoveRoomUser(int userId)
    {
        // TODO::user�� ó���� �°� �ٲٱ�
        foreach (RoomUserSlot user in userSlots)
        {
            if (user.playerId == userId)
            {
                user.EmptyRoomUser();
                break;
            }
        }

        users.RemoveAll(obj => obj.loginId == userId);
        for (int i = 0; i < userSlots.Count; ++i)
        {
            UserData userInfo = users.Count > i ? users[i] : null;
            userSlots[i].SetRoomUser(userInfo);
        }
    }
    #endregion

    #region �غ�
    private async void OnClickReadyButton()
    {
        buttonReady.interactable = false;

        if (isReady)
        {
            bool isSuccess = await CancelReadyAsync();
            if (isSuccess)
            {
                //isReady = false; �������� ����
                UpdateButtonUI("Ready", Color.grey, true);
            }
            else
            {
                Debug.Log("�غ���� ����");
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
                Debug.Log("�غ�Ϸ� ����");
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
        // ������ �غ� ��Ŷ ������
        //GamePacket packet = new();
        //packet.GamePrepareRequest = new()
        //{
        //    SessionId = GameManager.Instance.GetPlayerId()
        //    IsReady = true;
        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("������ �غ� �Ϸ� ��Ŷ ���� ��...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("�غ� �Ϸ� ��Ŷ ���� ����");

        return isSuccess ? true : false;
    }

    private async Task<bool> CancelReadyAsync()
    {
        readyTcs = new();

        // ������ �غ���� ��Ŷ ������
        //GamePacket packet = new();
        //packet.GamePrepareRequest = new()
        //{
        //    SessionId = GameManager.Instance.GetPlayerId()
        //    IsReady = false;
        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("������ �غ� ��� ��Ŷ ���� ��...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("�غ� ��� ��Ŷ ���� ����");

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
    /// S2C_GamePrepareNotification �� ���� �� ȣ��
    /// �غ�� �غ���� ��� ����.
    /// </summary>
    public void SetUserReady(int sessionId, bool isReady, int state)
    {
        bool isError = true;
        foreach(RoomUserSlot user in userSlots)
        {
            if(user.playerId == sessionId)
            {
                user.CheckReadyState(isReady, isHost);
                readyCount = isReady ? readyCount++ : readyCount--;
                this.state = (RoomStateType)state;
                Debug.Log($"���� ���� ����: {this.state}");

                if (readyCount == 4 ? true : false)
                {
                    buttonStart.interactable = true;
                    // TODO::��ư�̹��� ���� �� ����
                    Debug.Log("��� ������ �غ� �Ϸ�. ���۹�ư Ȱ��ȭ");
                }
                else
                {
                    buttonStart.interactable = false;
                    Debug.Log("���� �غ���� ���� ������ �ֽ��ϴ�.");
                }
                isError = false;
                break;
            }            
        }
        if (isError) Debug.LogError($"userSlots�� {sessionId}�� �����ϴ�.");
    }
    #endregion

    #region ����
    /// <summary>
    /// C2S_GameStartRequest
    /// ������ GameStart ��ư�� ���� ������ ���ӽ��� ��û�� ������. ������� �ε�Ǵ°� S2C_GameStartNotification �˸����� �����ҰŴ�.
    /// </summary>
    private void OnClickGameStartButton()
    {
        // TODO:: ������ ���ӽ��� ��Ŷ ������
        //GamePacket packet = new();
        //packet.GameStartRequest = new()
        //{
        //   SessionId = GameManager.Instance.PlayerId;
        //};
        //SocketManager.Instance.OnSend(packet);
    }

    /// <summary>
    /// S2C_GameStartNotification
    /// </summary>
    public async void GameStart()
    {
        await CountDownAsync(3);
        await UIManager.Show<UIFadeScreen>("FadeOut");
        invisibleWall.SetActive(false);

        // TODO:: ����� �ε�
        SceneManager.LoadScene("BoardScene");
    }
    #endregion

    #region GameSetting
    // �ִ� �� ������ �����Ϸ��� �˸� ��Ŷ�� �ʿ���.
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
        Debug.Log($"�ִ� �� : {maxTurn}");
        OnClickGameStartButton();
    }

    public void ButtonReady()
    {
        OnClickReadyButton();
    }
    #endregion

    #region ���� ����       

    // S2C_LeaveRoomResponse  
    // C2S_LeaveRoomRequest�� ���� ��������    
    // ������ �����ưUI�� ������
    public async void LeaveRoom()
    {
        //leaveRoomTcs = new();

        //GamePacket packet = new();
        //packet.C2S_LeaveRoomRequest = new()
        //{
        //    PlayerId = playerId
        //};
        //SocketManager.Instance.OnSend(packet);

        //bool isSuccess = await leaveRoomTcs.Task;

        //if (isSuccess)
        //{
        //    UIManager.Hide<UIRoom>();
        //    await UIManager.Show<UILobby>();
        //}
        //else
        //{
        //    Debug.LogError("������ ��Ž���. �泪���� ����");
        //}
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

    #region ���ϸŴ����� �ۼ��� �޼���    

    // �κ񿡼� C2S_JoinRoomRequest ��û�� ������ S2C_JoinRoomResponse ���� �� �����ִ� �޼��带 ȣ���Ѵ�.   
    // �κ񿡼� ������� �ִ� �濡 ������ �� S2C_JoinRoomResponse ����ް� ȣ���Ѵ�.
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
    /// SocketManager. ���ο� ������ ���濡 Join���� �� ���� ���� �����鿡�� �˸�.
    /// </summary>
    /// <param name="gamePacket"></param>
    public void JoinRoomNotification(GamePacket gamePacket)
    {
       //var response = gamePacket.JoinRoomNotification;
       //UIManager.Get<UIRoom>().AddRoomUser(response.JoinUser);
    }

    /// <summary>
    /// S2C_LeaveRoomResponse
    /// </summary>
    /// <param name="gamePacket"></param>
    public void LeaveRoomResponse(GamePacket gamePacket)
    {
        //var response = gamePacket.LeaveRoomResponse;
        //if(UIManager.Get<UIRoom>().leaveRoomTcs.TrySetResult(response.Success) )
        //{
        //    Debug.Log("Leave Room Success");
        //}
        //else
        //{
        //    // TODO:: FailCode�� �´� �˸��ٲٱ�
        //    Debug.LogError($"FailCode : {response.FailCode}");
        //}
    }

    /// <summary>
    /// S2C_LeaveRoomNotification 
    /// </summary>
    /// <param name="gamePacket"></param>
    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        //    var response = gamePacket.LeaveRoomNotification;
        //    UIManager.Get<UIRoom>().RemoveRoomUser(response.UserId);
    }

    /// <summary>
    /// S2C_GamePrepareResponse 
    /// </summary>
    /// <param name="packet"></param>
    public void GamePrepareResponse(GamePacket packet)
    {
        //var response = packet.GamePrepareResponse;
        //if (UIManager.Get<UIRoom>().readyTcs.TrySetResult(response.Success))
        //{
        //    UIManager.Get<UIRoom>().SetIsReady(response.IsReady);
        //}
        //else
        //{
        //    // TODO:: FailCode�� �´� �˸��ٲٱ�
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
        //UIManager.Get<UIRoom>().SetUserReady(response.SessionId, response.IsReady, response.State);
    }

    /// <summary>
    /// S2C_GameStartNotification
    /// </summary>
    public void GameStartNotification(GamePacket packet)
    {
        //var response = packet.GameStartNotification;
        //if (response.Success)
        //{
        //    UIManager.Get<UIRoom>().GameStart();
        //}
        //else
        //{
        //    Debug.LogError($"FailCode : {response.FailCode}");
        //}
    }


    #endregion
}