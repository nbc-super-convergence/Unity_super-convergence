﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

    private Dictionary<string, RoomPrefab> roomMap = new();

    public override void Opened(object[] param)
    {
        /* 로비 입장 Packet
         * [0] : 닉네임.
         */

        //닉네임 설정하기
        //nameTxt.text = param[0].ToString();

        AddRoom("테스트룸1", 4, 10);
        AddRoom("TestRoom22", 1, 100);
    }

    #region 버튼 이벤트
    //Inspector: 방 새로 만들기 버튼
    public async void OnBtnMakeRoom()
    {
        //방 새로 만들기 팝업.
        var result = await UIManager.Show<UIMakeRoom>();
    }

    //Inspector: 로그아웃 버튼
    public void OnBtnLogout()
    {
        //Send: 로그아웃 신호.
        UIManager.Hide<UILobby>();
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

    //SetRoom 메서드에서 이벤트 등록.
    private async void OnBtnParticipate()
    {
        //Send : 참여할 방.
        await UIManager.Show<UIRoom>();
        UIManager.Hide<UILobby>();
    }
    #endregion

    #region Room 관리
    //테스트용. 추후 서버와 연결 후 삭제.
    class RoomInfo
    {
        public string name;
        public int participant;
        public int ping;
    }

    private void SetRoomList(RoomInfo[] roomList)
    {
        HashSet<string> currentRoomNames = new HashSet<string>(roomMap.Keys);

        foreach (RoomInfo info in roomList)
        {
            if (roomMap.ContainsKey(info.name))
            {//이미 존재하는 방 정보 업데이트
                roomMap[info.name].SetRoomInfo(info.name, info.participant, info.ping);
                currentRoomNames.Remove(info.name);
            }
            else
            {//새로운 방 추가
                AddRoom(info.name, info.participant, info.ping);
            }
        }

        foreach (string roomName in currentRoomNames)
        {//사라진 방 제거
            roomMap[roomName].participateBtn.onClick.RemoveListener(OnBtnParticipate);
            Destroy(roomMap[roomName].gameObject);
            roomMap.Remove(roomName);
        }
    }

    private void AddRoom(string name, int participant, int ping)
    {
        RoomPrefab room = Instantiate(roomObj, roomParent.transform).GetComponent<RoomPrefab>();
        roomMap[name] = room;
        room.SetRoomInfo(name, participant, ping);
        room.participateBtn.onClick.AddListener(OnBtnParticipate);
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