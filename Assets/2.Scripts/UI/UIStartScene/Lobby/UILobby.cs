using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UILobby : UIBase
{
    [Header("Name")]
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private ChatSizeFitter fitter;

    [Header("roomList")]
    [SerializeField] private Transform roomParent;
    [SerializeField] private GameObject roomObj;
    [SerializeField] private TMP_InputField searchField;
    private Dictionary<string, RoomPrefab> roomMap;

    [Header("UserList")]
    [SerializeField] private Transform userParent;
    [SerializeField] private GameObject userObj;
    private Dictionary<string, UserPrefab> userMap;

    [Header("Button")]
    [SerializeField] private Button btnRefresh;

    private TaskCompletionSource<bool> lobbyTcs = null;
    private TaskCompletionSource<bool> roomTcs;
    private TaskCompletionSource<bool> userTcs;
    private float elapseTime;
    
    public override void Opened(object[] param)
    {
        SoundManager.Instance.PlayBGM(BGMType.Lobby);

        if (!SocketManager.Instance.isLobby)
        {
            LobbyJoinRequest();
            SocketManager.Instance.isLobby = true;
        }
        else
        {
            nameTxt.text = GameManager.Instance.myInfo.Nickname;
            fitter.UpdateSpeechBubbleSize();

        }
        roomMap = new();
        userMap = new();
        OnBtnRefresh();
    }

    private void Update()
    {
        elapseTime += Time.deltaTime;
        if (elapseTime >= 30f)
        {
            OnBtnRefresh();
            elapseTime = 0f;
        }
    }

    private async void LobbyJoinRequest()
    {
        GamePacket packet = new()
        {
            LobbyJoinRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId
            }
        };
        lobbyTcs = new();

        SocketManager.Instance.OnSend(packet);
        bool isSuccess = await lobbyTcs.Task;
        lobbyTcs = null;

        if (!isSuccess)
        {
            Debug.LogError($"UILobby lobbyTcs : {isSuccess}");
            OnBtnLogout();
        }
        else
        {
            nameTxt.text = GameManager.Instance.myInfo.Nickname;
            fitter.UpdateSpeechBubbleSize();
        }
    }

    public enum eTcs
    {
        Default,
        Lobby,
        Room,
        User
    };
    public void TrySetTask(bool isSuccess, eTcs type)
    {
        switch (type)
        {
            case eTcs.Lobby:
                lobbyTcs.TrySetResult(isSuccess);
                break;
            case eTcs.Room:
                roomTcs.TrySetResult(isSuccess);
                break;
            case eTcs.User:
                userTcs.TrySetResult(isSuccess);
                break;
        }
    }

    #region 버튼 이벤트
    //Inspector: 방 새로 만들기 버튼
    public async void OnBtnMakeRoom()
    {
        //방 새로 만들기 팝업.
        await UIManager.Show<UIMakeRoom>();
    }

    //Inspector: 로그아웃 버튼
    public async void OnBtnLogout()
    {
        //Send: 로그아웃 신호.
        GamePacket packet = new()
        {
            LobbyLeaveRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId
            }
        };
        lobbyTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await lobbyTcs.Task;
        lobbyTcs = null;

        if (isSuccess)
        {
            SocketManager.Instance.isLobby = false;
            UIManager.Hide<UILobby>();

#pragma warning disable CS4014
            UIManager.Show<UILogin>();
#pragma warning restore CS4014
        }
        else
        {
            Debug.LogError($"UILobby sourceTcs : {isSuccess}");
        }
    }

    //Inspector: 방 검색 InputField
    public void OnSearchRoomInput()
    {
        string searchQuery = searchField.text;
        bool isNotValid = string.IsNullOrEmpty(searchQuery);

        foreach (var room in roomMap.Values)
        {
            room.gameObject.SetActive(isNotValid);
        }
        if (isNotValid) { return; }

        var filteredRooms = roomMap
            .Where(pair => pair.Key.ToLower().Contains(searchQuery.ToLower()))
            .OrderBy(pair => SearchPriority(pair.Key, searchQuery))
            .Select(pair => pair.Value)
            .ToList();

        foreach (RoomPrefab room in filteredRooms)
        {
            Transform roomTransform = room.transform;
            roomTransform.SetSiblingIndex(filteredRooms.IndexOf(room));
            room.gameObject.SetActive(true);
        }
    }

    //todo : while문 무한반복 방지
    //Inspector: 방 새로고침 
    public async void OnBtnRefresh()
    {
        btnRefresh.interactable = false;

        await WaitUntilAsync(() => lobbyTcs == null);

        GamePacket roomPacket = new()
        {
            RoomListRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId
            }
        };
        roomTcs = new();
        SocketManager.Instance.OnSend(roomPacket);
        if (!await roomTcs.Task)
        {
            await UIManager.Show<UIError>("방 목록을 새로고침할 수 없습니다.");
        }
        roomTcs = null;

        await WaitUntilAsync(() => roomTcs == null);

        GamePacket userPacket = new()
        {
            LobbyUserListRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId
            }
        };
        userTcs = new();
        SocketManager.Instance.OnSend(userPacket);
        if (!await userTcs.Task)
        {
            await UIManager.Show<UIError>("사용자 목록을 새로고침할 수 없습니다.");
        }

        btnRefresh.interactable = true;
    }

    // 방 참가
    public async void TryJoinRoom(RoomData roomData, Button participateBtn)
    {
        participateBtn.interactable = false;
        GamePacket packet = new()
        {
            JoinRoomRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId,
                RoomId = roomData.RoomId
            }
        };
        lobbyTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await lobbyTcs.Task;
        lobbyTcs = null;

        if (isSuccess)
        {
        }
        participateBtn.interactable = true;
    }

    #endregion

    #region Room 관리
    // TODO:: RoomStateType state에 따라 참가불가능(클릭불가능)하게 만들기
    public void SetRoomList(RepeatedField<RoomData> roomList)
    {
        HashSet<string> currentRoomNames = new(roomMap.Keys);

        foreach (RoomData info in roomList)
        {
            if (roomMap.ContainsKey(info.RoomName))
            {//이미 존재하는 방 정보 업데이트
                roomMap[info.RoomName].SetRoomInfo(info.RoomName, info.Users.Count, GetRandomInt(20,40), info );
                currentRoomNames.Remove(info.RoomName);
            }
            else
            {//새로운 방 추가
                AddRoom(info.RoomName, info.Users.Count, GetRandomInt(20, 40), info);
            }
        }

        foreach (string roomName in currentRoomNames)
        {//사라진 방 제거
            Destroy(roomMap[roomName].gameObject);
            roomMap.Remove(roomName);
        }
    }
    
    private void AddRoom(string name, int participant, int ping, RoomData roomData)
    {
        RoomPrefab room = Instantiate(roomObj, roomParent.transform).GetComponent<RoomPrefab>();
        roomMap[name] = room;
        room.SetRoomInfo(name, participant, ping, roomData);
    }

    private int SearchPriority(string roomName, string query)
    {
        int index = roomName.IndexOf(query);
        if (index == 0) return 0;
        if (index > 0) return 1;
        return int.MaxValue;
    }
    #endregion

    #region User 관리
    public void SetUserList(RepeatedField<string> userList)
    {
        HashSet<string> currentUserNames = new(userMap.Keys);

        foreach (string name in userList)
        {
            if (userMap.ContainsKey(name))
            {//이미 존재하는 정보 업데이트
                userMap[name].SetName(name);
                currentUserNames.Remove(name);
            }
            else if (name != GameManager.Instance.myInfo.Nickname)
            {//새로운 유저 추가
                AddUser(name);
            }
        }

        foreach (string userName in currentUserNames)
        {//사라진 유저 제거
            if (userMap[userName] != null)
            {
                Destroy(userMap[userName].gameObject);
            }
            userMap.Remove(userName);
        }
    }

    private void AddUser(string name)
    {
        GameObject userInstance = Instantiate(userObj, userParent);
        if (userInstance != null)
        {
            if (userInstance.TryGetComponent(out UserPrefab user))
            {
                userMap[name] = user;
                user.SetName(name);
            }
            else
            {
                Debug.LogError("User Prefab 없음");
            }
        }
    }

    private async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 10)
    {
        while (!condition()) // 람다로 조건 확인
        {
            await Task.Delay(checkIntervalMs); // 주기적으로 확인
        }
    }
    #endregion

    // 패킷페이로드에 ping이 없어서 임시코드.
    private int GetRandomInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}