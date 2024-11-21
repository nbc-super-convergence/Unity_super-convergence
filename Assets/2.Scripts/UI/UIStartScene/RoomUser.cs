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

    private void Awake()
    {
        nickNameTMP = GetComponentInChildren<TMP_Text>();
        userImage = GetComponentInChildren<Image>();
    }

    public void SetNickname(string inputNickname)
    {
        sbNickname.Clear();
        sbNickname.Append(inputNickname);
        nickNameTMP.text = sbNickname.ToString();
    }
    public void SetImage()
    {
        
    }
}