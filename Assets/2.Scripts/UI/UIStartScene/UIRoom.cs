using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// �ӽ� Ŭ����
/// </summary>
public class UserInfo
{

}

public class UIRoom : UIBase
{
    [SerializeField] private bool isHost;
    private bool isReady = false;

    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonStart;

    private UnityAction onUserReadyChanged;
    [SerializeField] private UserInfo[] users= new UserInfo[4];
    private bool[] isReadyUsers = new bool[4];

    [SerializeField] private TMP_Text count;
    [SerializeField] private GameObject invisibleWall;
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
        buttonReady.onClick.AddListener(OnReadyButtonClick);
        onUserReadyChanged += TryActiveStartButton;

        SetIsHost();    //ReadyButton�� Test�غ����� SetIsHost(); �� �ּ�ó���ϱ�.
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
    

#region Host
    /// <summary>
    /// S2C_GamePrepareNotification �� ���� �� ȣ��
    /// </summary>
    /// <param name="userIndex"></param>
    public void SetUserReady(int userIndex)
    {
        if (userIndex >=0 && userIndex < isReadyUsers.Length)
        {
            isReadyUsers[userIndex] = true;

            onUserReadyChanged?.Invoke();
        }
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
        // ������ ���ӽ��� ��Ŷ ������
        //GamePacket packet = new();
        //packet.���ӽ��� = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);



        await CountDownAsync(3);
        await UIManager.Show<UIFadeScreen>("FadeOut");
        invisibleWall.SetActive(false);


        // ����� �ε�
    }
    #endregion


    #region !Host
    private async void OnReadyButtonClick()
    {
        buttonReady.interactable = false;

        if (isReady)
        {
            bool success = await CancelReadyAsync();
            if(success)
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
            bool success = await ReadyAsync();
            if (success)
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

    private async Task<bool> ReadyAsync()
    {
        // ������ �غ� ��Ŷ ������
        //GamePacket packet = new();
        //packet.�����غ� = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("������ �غ� �Ϸ� ��Ŷ ���� ��...");
        await Task.Delay(500);
        // ���� ����?: SocketManager.Instance.SendReadyPacketAsync();
        Debug.Log("�غ� �Ϸ� ��Ŷ ���� ����");
        return true;
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

    #endregion

    private async void BackLobby()
    {
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
        //await Task.Run(() => FadeScreen.Instance.FadeOut( () => UIManager.Hide<UIFadeScreen>()) );
    }

    #region Button
    public void ButtonBack()
    {
        BackLobby();
    }

    public void ButtonStart()
    {
        GameStart();
    }
    
#endregion

}
