using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomUserSlot : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // �����ƹ�Ÿ ǥ�ÿ�. � �������ε� �ٲ㵵 ��. �ʿ� ��������.    
    [SerializeField] private GameObject objReady;
    [SerializeField] private TMP_Text hostOrParticipant;

    public bool isReady = false;

    public int playerId;  // or userId or �ĺ������� ������


    private void Awake()
    {
        nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }
    
    public void SetRoomUser(UserData userData)
    {
        if (userData.nickname == null || userData.nickname == "")
        {
            nickNameTMP.text = "EMPTY";
            return;
        }        
        else
        {
            nickNameTMP.text = userData.nickname;
        }
        this.playerId = userData.loginId;
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