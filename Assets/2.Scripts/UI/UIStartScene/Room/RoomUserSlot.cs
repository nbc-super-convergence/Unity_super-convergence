using DG.Tweening;
using TMPro;
using UnityEngine;

public class RoomUserSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickNameTxt;
    [SerializeField] RectTransform userImg;
    [SerializeField] private GameObject[] icons;

    public bool isReady;
    public UserData userData;

    public void InitUserSlot()
    {
        userImg.anchoredPosition = new Vector3(0, -550, 0);
        nickNameTxt.text = "";
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }
    }

    public void AddUserSlot(UserData data, bool isOwner)
    {
        userData = data;
        nickNameTxt.text = data.Nickname;

        if (isOwner)
        {
            icons[0].SetActive(true); //방장 아이콘
            isReady = true;
        }
        else
        {
            icons[1].SetActive(true); //준비중 아이콘
            isReady = false;
        }

        userImg.DOAnchorPos(new Vector3(0, -110, 0), 1f)
            .SetEase(Ease.OutBack);
    }

    public void ReadyUserSlot(bool flag)
    {
        isReady = flag;
        icons[1].SetActive(!isReady); //준비중 아이콘
        icons[2].SetActive(isReady); //준비됨 아이콘
    }

    public void RemoveUserSlot()
    {
        nickNameTxt.text = "";
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }

        userImg.DOAnchorPos(new Vector3(0, -550, 0), 1f)
            .SetEase(Ease.InBack);
    }
}