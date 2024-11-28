using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUserSlot : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // �����ƹ�Ÿ ǥ�ÿ�. � �������ε� �ٲ㵵 ��. �ʿ� ��������.    
    [SerializeField] private GameObject objReady;
    [SerializeField] private TMP_Text hostOrParticipant;

    public bool isReady = false;

    public string loginId;  // or userId or �ĺ������� ������


    private void Awake()
    {
        nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }
    
    public void SetRoomUser(UserData userData)
    {
        if (userData.Nickname == null || userData.Nickname == "")
        {
            nickNameTMP.text = "EMPTY";
            return;
        }        
        else
        {
            nickNameTMP.text = userData.Nickname;
        }
        this.loginId = userData.LoginId;
    }

    public void SetImage()
    {
        
    }

    public void CheckReadyState(bool isReady, bool isHost)
    {
        hostOrParticipant.text = isHost ? "����" : "�غ�Ϸ�";
        this.isReady = isReady;
        objReady.SetActive(this.isReady ? true : false);
    }

    public void EmptyRoomUser()
    {
        nickNameTMP.text = "EMPTY";
        objReady.SetActive(false);        
    }

}