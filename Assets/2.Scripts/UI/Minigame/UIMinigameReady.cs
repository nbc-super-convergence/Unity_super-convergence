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

    [SerializeField] private TextMeshProUGUI gameTitle;
    [SerializeField] private GameObject[] gameDescription;
    [SerializeField] private ReadyPanels[] readyPanels;
    
    private eGameType gameType;

    public override void Opened(object[] param)
    {
        BoardManager.Instance.SetMiniGamePlaying(true);
        GameManager.OnPlayerLeft += PlayerLeftEvent;

        gameType = (eGameType)param[0]; // 게임 타입

        //게임 제목
        gameTitle.text = gameType switch
        {
            eGameType.GameIceSlider => "미끌미끌 얼음판",
            eGameType.GameBombDelivery => "폭탄 배달왔어요",
            eGameType.GameCourtshipDance => "댄스 스텝 챌린지",
            eGameType.GameDropper => "지하에서 디스코파티",
            eGameType.GameDart => "다트를 맞춰라",
            _ => "ERROR!!!",
        };

        //게임 설명
        gameDescription[(int)gameType - 1].SetActive(true);

        //ready 상태 초기화
        HashSet<int> usedColors = new HashSet<int>();
        foreach (var dic in GameManager.Instance.SessionDic)
        {
            int color = dic.Value.Color;
            usedColors.Add(color);

            readyPanels[color].outline.enabled = false;
            readyPanels[color].txt.text = "준비중...";
        }
        for (int i = 0; i < readyPanels.Length; i++)
        {
            if (!usedColors.Contains(i))
            {
                readyPanels[i].outline.enabled = false;
                readyPanels[i].txt.text = "오프라인";
                readyPanels[i].mask.SetActive(true);
            }
        }

        //R키(레디 입력) 대기
        StartCoroutine(WaitForReady());
    }

    public override void Closed(object[] param)
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
        gameDescription[(int)gameType - 1].SetActive(false);
    }

    public void SetReady(string sessionId, bool isMe = false)
    {
        if (isMe) sessionId = GameManager.Instance.myInfo.SessionId;
        int idx = GameManager.Instance.SessionDic[sessionId].Color;

        //준비상태 전환
        readyPanels[idx].outline.enabled = true;
        readyPanels[idx].txt.text = "준비 완료!";
    }

    private IEnumerator WaitForReady()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetReady(null, true);

                //Ready를 알리는 패킷 보내기
                GamePacket packet = new();

                switch (MinigameManager.gameType) {
                    case eGameType.GameIceSlider:
                        packet.IceGameReadyRequest = new()
                        {
                            SessionId = GameManager.Instance.myInfo.SessionId
                        };
                        break;
                    case eGameType.GameBombDelivery:
                        packet.BombGameReadyRequest = new()
                        {
                            SessionId = GameManager.Instance.myInfo.SessionId
                        };

                        break;
                    case eGameType.GameCourtshipDance:
                        packet.DanceReadyRequest = new()
                        {
                            SessionId = GameManager.Instance.myInfo.SessionId
                        };
                        break;
                    case eGameType.GameDropper:
                        packet.DropGameReadyRequest = new()
                        {
                            SessionId = GameManager.Instance.myInfo.SessionId
                        };
                        break;
                    case eGameType.GameDart:
                        packet.DartGameReadyRequest = new()
                        {
                            SessionId = GameManager.Instance.myInfo.SessionId
                        };
                        break;
                }

                SocketManager.Instance.OnSend(packet);
                break;
            }
            yield return null;
        }
    }

    private void PlayerLeftEvent(int color)
    {
        readyPanels[color].outline.enabled = false;
        readyPanels[color].txt.text = "오프라인";
        readyPanels[color].mask.SetActive(true);
    }
}