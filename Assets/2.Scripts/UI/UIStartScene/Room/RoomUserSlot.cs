using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUserSlot : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // 유저아바타 표시용. 어떤 유형으로든 바꿔도 됨. 필요 없을지도.    
    private bool isJoin = false;
    [SerializeField] private GameObject objReady;

    public int playerId;  // 서버와 같은 값
    private string nickNameEMPTY = "EMPTY";

    private void Awake()
    {
        nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }

    
    public void SetRoomUser(string inputNickname, int playerId)
    {
        if (inputNickname == null || inputNickname == "")
        {
            nickNameTMP.text = nickNameEMPTY;
            return;
        }        
        else
        {
            isJoin = true;
            nickNameTMP.text = inputNickname;
        }

        this.playerId = playerId;
    }
    public void SetImage()
    {
        
    }

    public void Ready(bool isReady)
    {
        if(isReady)
        {
            objReady.SetActive(true);
        }
        else
        {
            objReady.SetActive(false);
        }
    }

    public void LeaveRoomUser()
    {
        nickNameTMP.text = nickNameEMPTY;
        objReady.SetActive(false);
    }
}