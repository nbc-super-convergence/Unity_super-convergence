using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUserSlot : MonoBehaviour
{
    [SerializeField]private TMP_Text nickNameTMP;
    private Image userImage;    
    [SerializeField] private GameObject objReady;
    [SerializeField] private TMP_Text hostOrParticipant;

    public bool isReady = false;

    public string sessionId; 

    private void Awake()
    {
        //nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }
    
    public void SetRoomUser(UserData userData, int i)
    {
        if (userData == null || userData.Nickname == null || userData.Nickname == "")
        {
            nickNameTMP.text = "EMPTY";
            return;
        }        
        else
        {
            nickNameTMP.text = userData.Nickname;
        }
        this.sessionId = userData.SessionId;
        //SetImage(i);
        userImage.enabled = true;
    }

    public void SetImage(int color)
    {
        switch (color)
        {
            case 0: userImage.color = Color.red; break;
            case 1: userImage.color = Color.yellow; break;
            case 2: userImage.color = Color.blue; break;
            case 3: userImage.color = Color.green; break;
            default: userImage.color = Color.grey; break;
        }
    }

    public void CheckReadyState(bool isReady, string ownerId = null)
    {
        if (ownerId != null)
        {
            hostOrParticipant.text = (sessionId == ownerId) ? "방장" : "준비완료";
        }
        this.isReady = isReady;
        objReady.SetActive(this.isReady ? true : false);
    }

    public void EmptyRoomUser()
    {
        userImage.enabled = false;
        nickNameTMP.text = "EMPTY";
        objReady.SetActive(false);
        sessionId = "";
    }

}