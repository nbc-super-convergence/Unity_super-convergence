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
/// 임시 클래스
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

    [SerializeField] private TMP_Dropdown ddMaxTurn;
    private int[] turnOptions = { 10, 15, 20, 25, 30 };
    [Range(0, 4)] public int maxTurnValue = 0;
    private int maxTurn;

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

    private void Init()    //동적생성하면 Start가 Opened보다 늦게 실행된다.
    {
        SetIsHost();    // 호스트가 아닌 경우를 Test해보려면 이 줄을 주석처리하기.
        buttonReady.onClick.AddListener(OnReadyButtonClick);
        onUserReadyChanged += TryActiveStartButton;

        SetDropdown();
        SetUserReady(0);    // 방장은 자동 레디처리
    }

    private void Update()
    {
        // 키보드 입력으로 테스트 (임의로 준비 완료 처리)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetUserReady(0);        
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetUserReady(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetUserReady(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetUserReady(3);
    }

    /// <summary>
    /// 방을 만들고 내가 첫 유저면 isHost true.
    /// 이건 서버가 할 일?
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
    /// TODO:: S2C_GamePrepareNotification 를 받을 때 호출
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
                Debug.Log("준비취소 실패");
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
                Debug.Log("준비완료 실패");
                buttonReady.interactable = true;
            }
        }
    }

    private async Task<bool> ReadyAsync()
    {
        // 서버에 준비 패킷 보내기
        //GamePacket packet = new();
        //packet.게임준비 = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("서버로 준비 완료 패킷 전송 중...");
        await Task.Delay(500);
        // 실제 구현?: SocketManager.Instance.SendReadyPacketAsync();
        Debug.Log("준비 완료 패킷 전송 성공");
        return true;
    }

    private async Task<bool> CancelReadyAsync()
    {
        await Task.Delay(500);

        // 서버에 준비취소 패킷 보내기
        //GamePacket packet = new();
        //packet.게임준비취소 = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

        Debug.Log("서버로 준비 취소 패킷 전송 중...");
        await Task.Delay(500);
        // 실제 구현?: SocketManager.Instance.SendCancelReadyPacketAsync();

        Debug.Log("준비 취소 패킷 전송 성공");
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
        // TODO:: 방 나감 패킷 보내기

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
        Debug.Log($"최대 턴 : {maxTurn}");
        GameStart();
    }
    
#endregion

}
