using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RoomData   // Protocol.cs���� ��������� Ŭ����.
{
    public string name;
    public string number;
    public List<UserData> userDatas; // RoomData �ȿ� �������ִ� �ο� ������ �ִٰ� ������.
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
        SetIsHost();    // ȣ��Ʈ�� �ƴ� ��츦 Test�غ����� �� ���� �ּ�ó���ϱ�.
        buttonReady.onClick.AddListener(OnReadyButtonClick);
        onUserReadyChanged += TryActiveStartButton;

        SetDropdown();
        SetUserReady(0);    // ������ �ڵ� ����ó��
    }
    
    // �濡 ������ �ڵ����� ����
    // �� �̸� ���� ����
    // �� �������� ����
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
    /// ���� ����� ���� ù ������ isHost true.
    /// �̰� ������ �� ��?
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
    /// TODO:: S2C_GamePrepareNotification �� ���� �� ȣ��
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
            // TODO::��ư�̹��� ���� �� ����
            Debug.Log("��� ������ �غ� �Ϸ�. ���۹�ư Ȱ��ȭ");
        }
        else
        {
            buttonStart.interactable = false;
            Debug.Log("���� �غ���� ���� ������ �ֽ��ϴ�.");
            return;
        }
    }

    public bool IsReadyUsers()
    {
        // ��� ������ �����غ� ��Ƽ���̸� ������ true
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
        // TODO:: ������ ���ӽ��� ��Ŷ ������
        //GamePacket packet = new();
        //packet.���ӽ��� = new()
        //{
        // Maxturn = maxturn
        //};
        //SocketManager.Instance.OnSend(packet);



        await CountDownAsync(3);
        await UIManager.Show<UIFadeScreen>("FadeOut");
        invisibleWall.SetActive(false);


        // TODO:: ����� �ε�
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
                Debug.Log("�غ���� ����");
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
                Debug.Log("�غ�Ϸ� ����");
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
        // ������ �غ� ��Ŷ ������
        //GamePacket packet = new();
        //packet.GamePrepareRequest = new()
        //{
        //    PlayerId = GameManager.Instance.GetPlayerId()
        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("������ �غ� �Ϸ� ��Ŷ ���� ��...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("�غ� �Ϸ� ��Ŷ ���� ����");

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

        // ������ �غ���� ��Ŷ ������
        //GamePacket packet = new();
        //packet.�����غ���� = new()
        //{
        //  PlayerId = GameManager.Instance.GetPlayerId()
        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("������ �غ� ��� ��Ŷ ���� ��...");
        bool isSuccess = await readyTcs.Task;
        Debug.Log("�غ� ��� ��Ŷ ���� ����");
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
        // TODO:: �� ���� ��Ŷ ������


        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

    public void ButtonStart()
    {
        Debug.Log($"�ִ� �� : {maxTurn}");
        GameStart();
    }
    #endregion

    #region ���� ����


    // S2C_JoinRoomNotification �� ���� ��� ����
    public void SetRoomUserSlot(UserData data)
    {
        foreach (RoomUserSlot user in userSlots)    //TODO:: �ٲ� �ʿ䰡 �־��?
        {
            if (user == null)
            {
                user.SetRoomUser(data.nickname, data.userId);
                break;
            }
        }
    }

    #endregion

    #region ���� ����

    // C2S_LeaveRoomRequest
    // userId�� �����Ѵٴ� ��ȣ�� ������.
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
    // C2S_LeaveRoomRequest�� ���� ��������    
    // ������ �����ưUI�� ������
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
        //    Debug.LogError("������ ��Ž���. �泪���� ����");
        //}
    }

    // S2C_LeaveRoomNotification  
    // �ٸ������� ����˸��� ������...
    public void LeaveRoomUserNotification(int playerId)
    {
        // TODO::user�� ó���� �°� �ٲٱ�
        foreach (RoomUserSlot user in userSlots)
        {
            if (user.playerId == playerId)
            {
                // ����ó�� ����
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
            // TODO:: FailCode�� �´� �˸��ٲٱ�
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