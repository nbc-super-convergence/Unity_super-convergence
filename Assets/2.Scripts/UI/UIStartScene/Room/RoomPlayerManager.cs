using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class RoomPlayerManager : MonoBehaviour
{
    [SerializeField]private RoomUser[] users = new RoomUser[4];
    //private int currentUserCount = 0;

    [SerializeField] private StringBuilder[] sbUsers = new StringBuilder[4]; // 여기에 서버에서 받은 정보 저장하기

    //private UnityAction onUserChanged;


    #region 테스트코드
    string[] strings0 = { "손효재", "정승연", "탁혁재", "박인수" };
    string[] strings1 = { "박인수", "손효재", "정승연", "탁혁재" };
    string[] strings2 = { "손효재", "정승연", "", "" };

    int[] ints0 = { 6521, 4789, 35478, 1123 };
    int[] ints1 = { 1123, 6521, 4789, 35478 };
    int[] ints2 = { 6521, 4789, 0, 0 };


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ReceiveServer(strings0, ints0);
        if (Input.GetKeyDown(KeyCode.S)) ReceiveServer(strings1, ints1);
        if (Input.GetKeyDown(KeyCode.D)) ReceiveServer(strings2, ints2);
    }
    #endregion

    /// <summary>
    /// 서버로부터 리스폰스or 노티스 받을 때 실행될 메서드. 패킷의 정보를 저장, 갱신한다.
    /// S2C_JoinRoomNotification 을 받았을 때 실행
    /// </summary>
    /// <param name="nickNames"></param>
    public void ReceiveServer(string[] nickNames, int[] userId)
    {
        for (int i = 0; i < sbUsers.Length; i++)
        {
            if (sbUsers[i] == null)
            {
                sbUsers[i] = new StringBuilder();
            }
            sbUsers[i].Clear();
            sbUsers[i].Append(nickNames[i]);
            users[i].SetRoomUser(sbUsers[i].ToString(), userId[i]);
            users[i].SetImage();
        }

        Debug.Log($"현재 유저: {string.Join(" ", nickNames)}");
    }

    #region 유저 입장


    #endregion


    #region 유저 퇴장
    // 퇴장 시
    // S2C_LeaveRoomNotification
    // userId가 퇴장한다는 신호를 보낸다.

    #endregion

    public void ReadyThem(int index, bool isReady)
    {
        users[index].Ready(isReady);
    }
}