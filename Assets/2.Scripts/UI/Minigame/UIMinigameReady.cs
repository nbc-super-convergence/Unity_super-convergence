using System.Collections;
using TMPro;
using UnityEngine;

public class UIMinigameReady : UIBase
{
    private eGameType type;

    [SerializeField] private TextMeshProUGUI gameTitle;
    [SerializeField] private GameObject[] gameDescription;

    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject[] isReady;

    protected override void Awake()
    {
        base.Awake();
        foreach (var p in players)
        {
            p.SetActive(true);
        }
    }

    public override void Opened(object[] param)
    {
        type = (eGameType)param[0];

        //���� ����
        switch (type)
        {
            case eGameType.GameIceSlider:
                gameTitle.text = "�̲��̲� ������";
                break;
            case eGameType.GameBombDelivery:
                break;
            default:
                gameTitle.text = "ERROR!!!";
                break;
        }

        //���ӿ� �´� ����
        gameDescription[(int)type].SetActive(true);

        //ready ���� �ʱ�ȭ
        int i = 0;
        foreach (var dic in GameManager.Instance.SessionDic)
        {
            if (dic.Value == -1) players[i].SetActive(false);
            isReady[i].SetActive(false);
        }
            

        //RŰ �Է� ���
        StartCoroutine(WaitReady());
    }

    public override void Closed(object[] param)
    {
        gameDescription[(int)type].SetActive(false);
    }

    public void SetReady(string sessionId, bool isMe = false)
    {
        if (isMe) 
            sessionId = GameManager.Instance.myInfo.SessionId;
        int idx = GameManager.Instance.SessionDic[sessionId];
        isReady[idx].SetActive(true);
    }

    private IEnumerator WaitReady()
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

                break;
            }
            yield return null;
        }
    }
}