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

    [Header("roomList")]
    [SerializeField] private Transform roomParent;
    [SerializeField] private TMP_InputField searchField;

    [Header("Prefabs")]
    [SerializeField] private GameObject roomObj;
    [SerializeField] private GameObject myChatObj;
    [SerializeField] private GameObject otherChatObj;

    [Header("Button")]
    [SerializeField] private Button btnRefresh;

    private Dictionary<string, RoomPrefab> roomMap = new();

    private TaskCompletionSource<bool> sourceTcs;

    public override void Opened(object[] param)
    {
        if (!SocketManager.Instance.isLobby)
        {
            LobbyJoinRequest();
            SocketManager.Instance.isLobby = true;
        }
        else
        {
            nameTxt.text = GameManager.Instance.myInfo.Nickname;
        }

        //AddRoom("테스트룸1", 4, 10);
        //AddRoom("TestRoom22", 1, 100);
    }
    private async void LobbyJoinRequest()
    {
        GamePacket packet = new();
        packet.LobbyJoinRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId
        };
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await sourceTcs.Task;
        if(isSuccess)
        {
            //닉네임 설정하기
            nameTxt.text = GameManager.Instance.myInfo.Nickname;
        }
        else
        {
            Debug.LogError($"UILobby sourceTcs : {isSuccess}");
        }
    }

    public void TrySetTask(bool isSuccess)
    {
        bool boolll = sourceTcs.TrySetResult(isSuccess);
        Debug.Log(boolll ? "성공" : "실패");
    }

    #region 버튼 이벤트
    //Inspector: 방 새로 만들기 버튼
    public async void OnBtnMakeRoom()
    {
        //방 새로 만들기 팝업.
        var result = await UIManager.Show<UIMakeRoom>();
    }

    //Inspector: 로그아웃 버튼
    public async void OnBtnLogout()
    {
        //Send: 로그아웃 신호.
        GamePacket packet = new();
        packet.LobbyLeaveRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId
        };
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {
            SocketManager.Instance.isLobby = false;
            UIManager.Hide<UILobby>();
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

    //Inspector: 방 새로고침
    public async void OnBtnRefresh()
    {
        btnRefresh.interactable = false;

        GamePacket packet = new();
        packet.RoomListRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId
        };
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {
        }
        btnRefresh.interactable = true;
    }

    // 방 참가
    public async void TryJoinRoom(RoomData roomData, Button participateBtn)
    {
        participateBtn.interactable = false;
        GamePacket packet = new();
        packet.JoinRoomRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            RoomId = roomData.RoomId
        };
        sourceTcs = new();
        SocketManager.Instance.OnSend(packet);

        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {
        }
        participateBtn.interactable = true;
    }

    //SetRoom 메서드에서 이벤트 등록.
    private async void OnBtnParticipate()
    {
        //Send : 참여할 방.
        await UIManager.Show<UIRoom>();
        UIManager.Hide<UILobby>();
    }
    #endregion

    #region Room 관리

    // 새로고침
    // TODO:: RoomStateType state에 따라 참가불가능(클릭불가능)하게 만들기
    public void SetRoomList(RepeatedField<RoomData> roomList)
    {
        HashSet<string> currentRoomNames = new HashSet<string>(roomMap.Keys);

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
            //roomMap[roomName].participateBtn.onClick.RemoveListener(OnBtnParticipate);
            Destroy(roomMap[roomName].gameObject);
            roomMap.Remove(roomName);
        }
    }
    // 패킷페이로드에 ping이 없어서 임시코드.
    private int GetRandomInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }


    private void AddRoom(string name, int participant, int ping, RoomData roomData)
    {
        RoomPrefab room = Instantiate(roomObj, roomParent.transform).GetComponent<RoomPrefab>();
        roomMap[name] = room;
        room.SetRoomInfo(name, participant, ping, roomData);
        //room.participateBtn.onClick.AddListener(OnBtnParticipate);
    }

    private int SearchPriority(string roomName, string query)
    {
        int index = roomName.IndexOf(query);
        if (index == 0) return 0;
        if (index > 0) return 1;
        return int.MaxValue;
    }
    #endregion

    #region Chat
    //채팅 (후순위)
    //유저목록 (후순위)
    #endregion
}