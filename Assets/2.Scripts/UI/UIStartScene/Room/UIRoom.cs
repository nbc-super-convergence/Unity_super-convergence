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
    [SerializeField] private Button[] kickUsers;
    
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
        SoundManager.Instance.PlayBGM(BGMType.Room);
        roomUI.blocksRaycasts = true;

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
        
        /*버튼 상태 초기화*/
        InitActiveBtn();

        /*Dictionary 초기화*/
        ResetSessionDics(data.Users);

        UpdateUserSlots();

        if (roomData.State == RoomStateType.Prepare)
        {
            StartBtn.interactable = true;
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
        HashSet<string>  prevSessionIDs = new(roomData.Users.Select(user => user.SessionId));
        HashSet<string> curSessionIDs = new(data.Users.Select(user => user.SessionId));

        roomData = data;

        if (isJoin)
        {
            ResetSessionDics(data.Users);
            string memberId = curSessionIDs.Except(prevSessionIDs).ToList()[0];
            int memberIdx = GameManager.Instance.SessionDic[memberId].Color;

            userSlots[memberIdx].AddUserSlot(roomData.Users[memberIdx],
                memberId == roomData.OwnerId);
            userSlots[memberIdx].ReadyUserSlot(false);

            Debug.LogWarning(data.State);
        }
        else
        {
            string memberId = prevSessionIDs.Except(curSessionIDs).ToList()[0];
            int memberIdx = GameManager.Instance.SessionDic[memberId].Color;

            for (int i = memberIdx; i < userSlots.Count; i++)
            {
                userSlots[i].RemoveUserSlot();
            }
            ResetSessionDics(roomData.Users);

            if (IsHost) InitActiveBtn(); //Host = StartBtn 활성화

            UpdateUserSlots(memberIdx);

            Debug.Log(data.State);
        }
    }

    public void OnKickEvent(bool isKicked, RoomData data)
    {
        if (isKicked)
        {
#pragma warning disable CS4014
            UIManager.Show<UIError>("방장에 의해 추방당했습니다.\n로비로 돌아갑니다...");
            UIManager.Hide<UIRoom>();
            UIManager.Show<UILobby>();
#pragma warning restore CS4014
        }
        else
        {
            OnRoomMemberChange(data, false); //kick = 나가기 처리.
        }
    }
    #endregion
    
    #region Ready Event
    public void SetUserReady(bool isReady, string sessionId = null, RoomStateType roomState = 0)
    {
        if (sessionId == null)
        {
            sessionId = GameManager.Instance.myInfo.SessionId;
            ToggleReadyBtn(isReady);
        }

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
        roomUI.blocksRaycasts = false;
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
            int currentNum = i;
            sequence.AppendCallback(() => countDownTxt.text = currentNum.ToString());
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
        Debug.Log(userSlots[GameManager.Instance.myInfo.Color].isReady);
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

    public void OnKickBtn(int idx)
    {
        if (!(1 <= idx && idx <= 3))
        {
            return;
        }

#pragma warning disable CS4014
        UIManager.Show<UIKick>(idx, userSlots[idx].userData.Nickname);
#pragma warning restore CS4014
    }

    private void InitActiveBtn()
    {
        foreach (var item in kickUsers)
        {
            item.interactable = false;
        }

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

    private void ToggleReadyBtn(bool isReady)
    {
        readyBtn.gameObject.SetActive(!isReady);
        cancelReadyBtn.gameObject.SetActive(isReady);
    }
    #endregion

    #region 보조 함수
    public void TrySetTask(bool isSuccess)
    {
        roomTcs.TrySetResult(isSuccess);
    }

    private void ResetSessionDics(RepeatedField<UserData> Users)
    {
        GameManager.Instance.SessionDic.Clear();
        int num = 0;
        foreach (var user in Users)
        {
            GameManager.Instance.SessionDic.Add(user.SessionId, new UserInfo(user.SessionId, user.Nickname, num, num));

            if (GameManager.Instance.myInfo.SessionId == user.SessionId)
            {
                GameManager.Instance.SetMyInfo(GameManager.Instance.SessionDic[GameManager.Instance.myInfo.SessionId]);
            }
            num++;
        }

        if (IsHost)
        {
            for (int i = 0; i < Users.Count - 1; i++)
            {
                kickUsers[i].interactable = true;
            }
        }
    }

    private void UpdateUserSlots(int idx = 0)
    {
        /*준비된 유저 HashSet*/
        HashSet<string> readyUserId = new HashSet<string>();
        readyUserId.UnionWith(roomData.ReadyUsers);

        /*유저 슬롯 세팅*/
        for (int i = idx; i < roomData.Users.Count; i++)
        {
            if (roomData.Users[i].SessionId == roomData.OwnerId)
            {
                userSlots[i].AddUserSlot(roomData.Users[i], true);
            }
            else
            {
                userSlots[i].AddUserSlot(roomData.Users[i], false);

                if (readyUserId.Contains(roomData.Users[i].SessionId))
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

    public void KickUser(int idx)
    {
        GamePacket gamePacket = new()
        {
            RoomKickRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId,
                TargetSessionId = userSlots[idx].userData.SessionId,
            }
        };
        roomTcs = new();
        SocketManager.Instance.OnSend(gamePacket);
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

        return await roomTcs.Task;
    }
    #endregion
}