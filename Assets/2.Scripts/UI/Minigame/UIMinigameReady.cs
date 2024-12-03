using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameReady : UIBase
{
    [Serializable]
    private class ReadyPanels
    {
        public Outline outline;
        public TextMeshProUGUI txt;
        public GameObject mask;
    }

    private eGameType gameType;

    [SerializeField] private TextMeshProUGUI gameTitle;
    [SerializeField] private GameObject[] gameDescription;

    [SerializeField] private ReadyPanels[] readyPanels;

    public override void Opened(object[] param)
    {
        GameManager.OnPlayerLeft += PlayerLeftEvent;

        gameType = (eGameType)param[0]; // ���� Ÿ��

        //���� ����
        gameTitle.text = gameType switch
        {
            eGameType.GameIceSlider => "�̲��̲� ������",
            eGameType.GameBombDelivery => "��ź ��޿Ծ��",
            _ => "ERROR!!!",
        };

        //���� ����
        gameDescription[(int)gameType - 1].SetActive(true);

        //ready ���� �ʱ�ȭ
        HashSet<int> usedColors = new HashSet<int>();
        foreach (var dic in GameManager.Instance.SessionDic)
        {
            int color = dic.Value.Color;
            usedColors.Add(color);

            readyPanels[color].outline.enabled = false;
            readyPanels[color].txt.text = "�غ���...";
        }
        for (int i = 0; i < readyPanels.Length; i++)
        {
            if (!usedColors.Contains(i))
            {
                readyPanels[i].outline.enabled = false;
                readyPanels[i].txt.text = "��������";
                readyPanels[i].mask.SetActive(true);
            }
        }

        //RŰ(���� �Է�) ���
        StartCoroutine(WaitForReady());
    }

    public override void Closed(object[] param)
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
        gameDescription[(int)gameType].SetActive(false);
    }

    private IEnumerator WaitForReady()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetReady(null, true);

                //Ready�� �˸��� ��Ŷ ������
                GamePacket packet = new()
                {
                    IceGameReadyRequest = new()
                    {
                        SessionId = GameManager.Instance.myInfo.SessionId
                    }
                };
                SocketManager.Instance.OnSend(packet);
                Debug.Log(packet); //TODO : ��Ŷ �� ���������� �׽�Ʈ��. ����.
                break;
            }
            yield return null;
        }
    }

    public void SetReady(string sessionId, bool isMe = false)
    {
        if (isMe) sessionId = GameManager.Instance.myInfo.SessionId;
        int idx = GameManager.Instance.SessionDic[sessionId].Color;

        //�غ���� ��ȯ
        readyPanels[idx].outline.enabled = true;
        readyPanels[idx].txt.text = "�غ� �Ϸ�!";
    }

    private void PlayerLeftEvent(int color)
    {
        readyPanels[color].outline.enabled = false;
        readyPanels[color].txt.text = "��������";
        readyPanels[color].mask.SetActive(true);
    }
}