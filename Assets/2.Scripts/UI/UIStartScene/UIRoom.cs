using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIRoom : UIBase
{
    private bool isHost;
    // �غ��ư ��۹�ư
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
        // �غ�Ϸ� ��Ŷ�� ���������� �̺�Ʈ ȣ��
        // ��� ������ �����غ� ��Ƽ���̸� ������ true
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
        // ������ ���ӽ��� ��Ŷ ������
        //GamePacket packet = new();
        //packet.���ӽ��� = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

    }
#endregion

#region !Host
    private void Ready()
    {
        // ������ ���� ��Ŷ ������
        //GamePacket packet = new();
        //packet.�����غ� = new()
        //{

        //};
        //SocketManager.Instance.OnSend(packet);

        // ��ư ��Ȱ��ȭ, ��ư�̹���, ��ȣ�ۿ� �ٲٱ�

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
