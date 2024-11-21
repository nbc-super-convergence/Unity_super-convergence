using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUser : MonoBehaviour
{
    private TMP_Text nickNameTMP;
    private Image userImage;    // �����ƹ�Ÿ ǥ�ÿ�. � �������ε� �ٲ㵵 ��. �ʿ� ��������.
    private StringBuilder sbNickname = new();
    private bool isJoin = false;
    [SerializeField] private GameObject objReady;

    public int playerId;

    private void Awake()
    {
        nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }

    
    public void SetNickname(string inputNickname)
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