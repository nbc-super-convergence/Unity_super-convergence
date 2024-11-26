using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �ӽ� Ŭ����
/// </summary>
public class UserInfo
{

}
public class RoomData
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

    [Header("Room Info")]
    private RoomData roomData;
    [SerializeField] private TMP_Text roomNumber;
    [SerializeField] private TMP_Text roomName;

    private UnityAction onUserReadyChanged;
    [SerializeField] private UserInfo[] users= new UserInfo[4];
    private bool[] isReadyUsers = new bool[4];
    public TaskCompletionSource<bool> readyTcs;

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

    public override void Opened(object[] param)
    {
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
    }

    private void Init()    //���������ϸ� Start�� Opened���� �ʰ� ����ȴ�.
    {
        SetIsHost();    // ȣ��Ʈ�� �ƴ� ��츦 Test�غ����� �� ���� �ּ�ó���ϱ�.
        buttonReady.onClick.AddListener(OnReadyButtonClick);
        onUserReadyChanged += TryActiveStartButton;

        SetDropdown();
        SetUserReady(0);    // ������ �ڵ� ����ó��

    }

    private void Update()
    {
        // Ű���� �Է����� �׽�Ʈ (���Ƿ� �غ� �Ϸ� ó��)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetUserReady(0);        
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetUserReady(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetUserReady(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetUserReady(3);
    }


    public void SetRoomInfo(RoomData data)
    {
        roomData = data;
        roomNumber.text = (data.number != null) ? $"No. {data.number.ToString()}" : "";
        roomName.text = (data.name != null) ? data.name.ToString() : "";
    }

    // �κ񿡼� ���� ������ ��û�ϰ� ������ �濡 ���� �Ѵٰ� �����ϰ� �ۼ�.
    


    /// <summary>
    /// ���� ����� ���� ù ������ isHost true.
    /// �̰� ������ �� ��?
    /// </summary>
    public void SetIsHost()
    {
        if (users[0] == null)
        {
            users[0] = new UserInfo();
            isHost = true;
        }
        else
        {
            for ( int i = 1; i < users.Length; ++i )
            {
                if (users[i] == null)
                {
                    users[i] = new UserInfo();
                    break;
                }
            }
        }        
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

    #region Host

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





    #endregion


    #region !Host


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

        // ������ �غ���� ��Ŷ ������
        //GamePacket packet = new();
        //packet.�����غ���� = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("������ �غ� ��� ��Ŷ ���� ��...");
        await Task.Delay(500);
        // ���� ����?: SocketManager.Instance.SendCancelReadyPacketAsync();

        Debug.Log("�غ� ��� ��Ŷ ���� ����");
        return true;
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

    private void UpdateReadyState(int userIndex)
    {
        var item = GetComponent<RoomPlayerManager>();
        item.ReadyThem(userIndex, isReady);
    }
    

    #endregion

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

    private async void BackLobby()
    {
        // TODO:: �� ���� ��Ŷ ������

        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
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

    #region Button
    public void ButtonBack()
    {
        BackLobby();
    }

    public void ButtonStart()
    {
        Debug.Log($"�ִ� �� : {maxTurn}");
        GameStart();
    }

    #endregion



    #region ���ϸŴ����� �ۼ��� �޼���
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


    #endregion
}