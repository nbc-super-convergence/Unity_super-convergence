using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIRoom : UIBase
{
    private bool isHost;
    // 준비버튼 토글버튼
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonStart;

    private UnityAction action;
    private bool[] isReadyUsers = new bool[3];

    public override void Opened(object[] param)
    {
        if(isHost)
        {
            buttonStart.gameObject.SetActive(true);
            buttonReady.gameObject.SetActive(false);
            buttonStart.interactable = false;
        }
        else
        {
            buttonReady.gameObject.SetActive(true);
            buttonStart.gameObject.SetActive(false);
        }
    }

#region Host
    public bool IsReadyUsers()
    {
        // 준비완료 패킷을 받을때마다 이벤트 호출
        // 모든 유저의 게임준비 노티파이를 받으면 true
        for(int i = 0; i < isReadyUsers.Length; i++)
        {
            if (!isReadyUsers[i])
            {
                return false;
            }
        }
        return true;
    }

    private void ActiveStartButton()
    {
        buttonStart.interactable = true;
    }

    private void GameStart()
    {
        // 서버에 게임시작 패킷 보내기
        //GamePacket packet = new();
        //packet.게임시작 = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

    }
#endregion

#region !Host
    private void Ready()
    {
        // 서버에 레디 패킷 보내기
        //GamePacket packet = new();
        //packet.게임준비 = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

        // 버튼 비활성화, 버튼이미지, 상호작용 바꾸기

    }
#endregion
    private async void BackLobby()
    {
        UIManager.Hide<UIRoom>();
        await UIManager.Show<UILobby>();
    }

#region Button
    public void ButtonBack()
    {
        BackLobby();
    }

    public void ButtonReady()
    {
        Ready();
    }

    public void ButtonStart()
    {
        GameStart();
    }
    
#endregion


}
