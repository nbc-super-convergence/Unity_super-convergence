using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPrefab : MonoBehaviour
{
    private enum iconType
    {
        publicRoom = 0,
        privateRoom,
        networkGood,
        networkBad
    }

    [SerializeField] private Sprite[] icons;

    [SerializeField] private Image roomIcon;
    [SerializeField] private TextMeshProUGUI roomTxt;
    [SerializeField] private TextMeshProUGUI participantTxt;
    [SerializeField] private Image pingIcon;
    [SerializeField] private TextMeshProUGUI pingTxt;

    public Button participateBtn;
    
    private RoomData roomData;

    public void SetRoomInfo(string roomName, int participant, int ping, RoomData roomData)
    {
        //room ���� ����
        roomIcon.sprite = icons[(int)iconType.publicRoom];
        //room �̸�
        roomTxt.text = roomName;
        //room ������ ��
        participantTxt.text = participant.ToString();

        //�� ������ & ��ġ
        if (ping < 30)
        {
            pingIcon.sprite = icons[(int)iconType.networkGood];
        }
        else
        {
            pingIcon.sprite = icons[(int)iconType.networkBad];
        }
        pingTxt.text = ping.ToString();

        this.roomData = roomData;
    }

    public void OnBtnParticipate()
    {
        UIManager.Get<UILobby>().TryJoinRoom(this.roomData, participateBtn);     
    }
}
