using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUserSlot : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // �����ƹ�Ÿ ǥ�ÿ�. � �������ε� �ٲ㵵 ��. �ʿ� ��������.    
    private bool isJoin = false;
    [SerializeField] private GameObject objReady;

    public int playerId;  // ������ ���� ��
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