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
    private Image userImage;    // 유저아바타 표시용. 어떤 유형으로든 바꿔도 됨. 필요 없을지도.    
    [SerializeField] private GameObject objReady;
    [SerializeField] private TMP_Text hostOrParticipant;

    public bool isReady = false;

    public int playerId;  // or userId or 식별가능한 고유값


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
        hostOrParticipant.text = isHost ? "방장" : "준비완료";
        this.isReady = isReady;
        objReady.SetActive(this.isReady ? true : false);
    }

    public void EmptyRoomUser()
    {
        nickNameTMP.text = "EMPTY";
        objReady.SetActive(false);        
    }

}