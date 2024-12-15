using DG.Tweening;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoom : UIBase
{
    [SerializeField] private List<RoomUserSlot> userSlots;
    
    [Header("Room Info")]
    [SerializeField] private CanvasGroup roomUI;
    [SerializeField] private TextMeshProUGUI roomName;
    private RoomData roomData;
    public bool IsHost => roomData.OwnerId == GameManager.Instance.myInfo.SessionId;
    public bool IsClientReady => userSlots[GameManager.Instance.myInfo.Color].isReady;

    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countDownTxt;
    
    [Header("Button")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button cancelReadyBtn;

    private TaskCompletionSource<bool> roomTcs;

    
    public override void Opened(object[] param)
    {
        foreach (var slot in userSlots)
        {
            slot.InitUserSlot();
        }

        if (param.Length == 1)
        {
            if (param[0] is RoomData data)
            {
                EnterRoom(data);
            }
            else
            {
                Debug.LogError("param parsing error : roomData");
                return;
            }
        }
        else
        {
            Debug.LogError("param length error");
            return;
        }   
    }

    //TODO : 위치 이동
    public override async void HideDirect()
    {
        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

    #region My Room Join / Leave
    private void EnterRoom(RoomData data)
    {
        /*방 정보 초기화*/
        roomData = data;
        roomName.text = data.RoomName;
        
        /*Dictionary 초기화*/
        ResetSessionDics(data.Users);

        /*버튼 상태 초기화*/
        InitActiveBtn();

        /*준비된 유저 HashSet*/
        HashSet<string> readyUserId = new HashSet<string>();
        readyUserId.UnionWith(data.ReadyUsers);

        /*유저 슬롯 세팅*/
        for (int i = 0; i < data.Users.Count; i++)
        {
            if (IsHost)
            {
                userSlots[i].AddUserSlot(data.Users[i], true);
            }
            else
            {
                userSlots[i].AddUserSlot(data.Users[i], false);
                
                if (readyUserId.Contains(data.Users[i].SessionId))
                {
                    userSlots[i].ReadyUserSlot(true);
                }
                else
                {
                    userSlots[i].ReadyUserSlot(false);
                }
            }
        }
    }

    public async void LeaveRoom()
    {
        GamePacket packet = new()
        {
            LeaveRoomRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId
            }
        };
        roomTcs = new();
        SocketManager.Instance.OnSend(packet);

        if (await roomTcs.Task)
        {
            UIManager.Hide<UIRoom>();
            await UIManager.Show<UILobby>();
        }
    }
    #endregion

    #region Other Room Join / Leave
    public void OnRoomMemberChange(RoomData data, bool isJoin)
    {
        string memberId = "";
        int memberIdx = -1;
        HashSet<string>  prevSessionIDs = new HashSet<string>(roomData.Users.Select(user => user.SessionId));
        HashSet<string> curSessionIDs = new HashSet<string>(data.Users.Select(user => user.SessionId));

        if (isJoin)
        {
            ResetSessionDics(data.Users);
            memberId = curSessionIDs.Except(prevSessionIDs).ToList()[0];
            memberIdx = GameManager.Instance.SessionDic[memberId].Color;

            userSlots[memberIdx].AddUserSlot(data.Users[memberIdx],
                memberId == data.OwnerId);
            userSlots[memberIdx].ReadyUserSlot(false);
        }
        else
        {
            memberId = prevSessionIDs.Except(curSessionIDs).ToList()[0];
            memberIdx = GameManager.Instance.SessionDic[memberId].Color;
            ResetSessionDics(data.Users);

            userSlots[memberIdx].RemoveUserSlot();
        }

        /*방 정보 업데이트*/
        roomData = data;
    }
    #endregion
    
    #region Ready Event
    public void SetUserReady(bool isReady, string sessionId = null, RoomStateType roomState = 0)
    {
        sessionId ??= GameManager.Instance.myInfo.SessionId;
        int readyIdx = GameManager.Instance.SessionDic[sessionId].Color;
        userSlots[readyIdx].ReadyUserSlot(isReady);

        if (IsHost)
        {
            roomData.State = roomState;
            if (roomData.State == RoomStateType.Prepare)
            {
                StartBtn.interactable = true;
            }
            else
            {
                StartBtn.interactable = false;
            }
        }
    }
    #endregion

    #region Game Start Event
    public async void GameStart()
    {
        //myInfo Color, Order 초기화.
        GameManager.Instance.SetMyInfo(GameManager.Instance.SessionDic[GameManager.Instance.myInfo.SessionId]); 

        roomUI.blocksRaycasts = true;
        await CountDownAsync(3);
        
        UIManager.SceneChangeTask = new();
        UIManager.Instance.LoadingScreen.OnLoadingEvent(UIManager.SceneChangeTask);
        GameManager.isGameStart = true;
    }
    private async Task CountDownAsync(int countNum)
    {
        countDownTxt.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        for (int i = countNum; i > 0; i--)
        {
            sequence.AppendCallback(() => countDownTxt.text = i.ToString());
            sequence.AppendInterval(1f); 
        }
        sequence.OnComplete(() => countDownTxt.gameObject.SetActive(false));
        await sequence.AsyncWaitForCompletion();
    }
    #endregion

    #region Button Events
    public void OnBackBtn()
    {
        LeaveRoom();
    }
    
    public void OnGameStartBtn()
    {
        GamePacket packet = new()
        {
            GameStartRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId,
                Turn = 10,
            }
        };
        SocketManager.Instance.OnSend(packet);
    }

    public async void OnReadyBtn()
    {
        readyBtn.interactable = false;

        if (IsClientReady && await ReadyAsync(false))
        {
            readyBtn.gameObject.SetActive(true);
            cancelReadyBtn.gameObject.SetActive(false);
        }
        else if (await ReadyAsync(true))
        {
            readyBtn.gameObject.SetActive(false);
            cancelReadyBtn.gameObject.SetActive(true);
        }

        readyBtn.interactable = true;
    }
    #endregion

    #region 보조 함수
    public void TrySetTask(bool isSuccess)
    {
        roomTcs.TrySetResult(isSuccess);
    }

    private void InitActiveBtn()
    {
        if (IsHost)
        {
            StartBtn.gameObject.SetActive(true);
            readyBtn.gameObject.SetActive(false);
            cancelReadyBtn.gameObject.SetActive(false);
            StartBtn.interactable = false;
        }
        else
        {
            readyBtn.gameObject.SetActive(true);
            StartBtn.gameObject.SetActive(false);
            cancelReadyBtn.gameObject.SetActive(false);
        }
    }

    private void ResetSessionDics(RepeatedField<UserData> Users)
    {
        GameManager.Instance.SessionDic.Clear();
        int num = 0;
        foreach (var user in Users)
        {
            GameManager.Instance.SessionDic.Add(user.SessionId, new UserInfo(user.SessionId, user.Nickname, num, num));
            num++;
        }
    }

    private async Task<bool> ReadyAsync(bool isReady)
    {
        GamePacket packet = new()
        {
            GamePrepareRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId,
                IsReady = isReady
            }
        };
        roomTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await roomTcs.Task;

        return isSuccess ? true : false;
    }
    #endregion
}