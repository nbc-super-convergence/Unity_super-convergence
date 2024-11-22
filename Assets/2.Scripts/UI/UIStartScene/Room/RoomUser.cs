using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUser : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // 유저아바타 표시용. 어떤 유형으로든 바꿔도 됨. 필요 없을지도.
    private StringBuilder sbNickname = new();
    private bool isJoin = false;
    [SerializeField] private GameObject objReady;

    public int userId;  // 서버와 같은 값

    private void Awake()
    {
        nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }

    
    public void SetRoomUser(string inputNickname, int userId)
    {
        if (inputNickname == null || inputNickname == "")
        {
            sbNickname.Clear();
            sbNickname.Append("EMPTY");
            nickNameTMP.text = sbNickname.ToString();
            return;
        }        
        else
        {
            isJoin = true;
            sbNickname.Clear();
            sbNickname.Append(inputNickname);
            nickNameTMP.text = sbNickname.ToString();
        }

        this.userId = userId;
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
}