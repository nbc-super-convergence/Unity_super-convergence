using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUserSlot : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // 유저아바타 표시용. 어떤 유형으로든 바꿔도 됨. 필요 없을지도.    
    [SerializeField] private GameObject objReady;
    [SerializeField] private TMP_Text hostOrParticipant;

    public bool isReady = false;

    public string loginId;  // or userId or 식별가능한 고유값


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