using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameDropper : UIBase
{
    [Header("Start")]
    [SerializeField] private TextMeshProUGUI StartCountTxt;
    [SerializeField] private TextMeshProUGUI descTxt;

    [Header("Move Input")]
    [SerializeField] private Button[] directionBtns;


    private GameDropperData gameData;
    private Coroutine StunCoroutine;

    public override void Opened(object[] param)
    {
        GameManager.OnPlayerLeft += PlayerLeftEvent;

        if (param.Length == 2)
        {
            if (param[0] is GameDropperData data)
            {
                gameData = data;
            }
            else
            {
                Debug.LogError("param parsing error : GameDropperData");
                return;
            }

            if (param[1] is long startTime)
            {
                StartCoroutine(StartCountDown(startTime));
            }
            else
            {
                Debug.LogError("param parsing error : startTime");
                return;
            }
        }
        else
        {
            Debug.LogError("param length error");
            return;
        }
    }

    public override void Closed(object[] param)
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
        descTxt.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator StartCountDown(long startdelay)
    {
        Sequence sequence = DOTween.Sequence();

        /*"Ready?" 연출*/
        StartCountTxt.gameObject.SetActive(true);
        StartCountTxt.text = "Ready?";
        StartCountTxt.color = new Color(StartCountTxt.color.r, StartCountTxt.color.g, StartCountTxt.color.b, 0);
        sequence.AppendInterval(0.2f);
        sequence.Append(StartCountTxt.DOFade(1, 0.5f)); 
        sequence.AppendInterval(0.8f);   
        sequence.Append(StartCountTxt.DOFade(0, 0.5f));

        yield return new WaitUntil(() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= startdelay); //시작시간 대기
        gameData.phase++; //바닥 없어짐
        StartCoroutine(MovableTime());

        //"Start!" 연출
        sequence.AppendCallback(() =>
        {
            StartCountTxt.text = "Start!";
            StartCountTxt.color = new Color(StartCountTxt.color.r, StartCountTxt.color.g, StartCountTxt.color.b, 0);
            StartCountTxt.transform.localScale = Vector3.one * 0.8f;
        });
        sequence.Append(StartCountTxt.DOFade(1, 0.5f));
        sequence.Join(StartCountTxt.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutQuad));
        sequence.AppendInterval(0.5f); 
        sequence.Append(StartCountTxt.DOFade(0, 0.3f));
        StartCountTxt.gameObject.SetActive(false);

        descTxt.transform.parent.gameObject.SetActive(true);
        DescTxtEffect(false);
    }

    public IEnumerator MovableTime()
    {
        EnableBtnInput();
        yield return new WaitForSeconds(9f);
        
        if (StunCoroutine != null)
        {
            StopCoroutine(StunCoroutine);
            StunCoroutine = null;
        }
        
        DisableBtnInput();

        DescTxtEffect(true);
    }

    public void RequestMove(int direction)
    {
        int curSlot = gameData.curSlot, nextSlot = -1;
        int rotation = 0;

        //방향버튼 유효성 검사
        switch(direction)
        {
            case 0:
                if (curSlot - 3 >= 0)
                {
                    nextSlot = curSlot - 3;
                    rotation = 180;
                }
                break;
            case 1:
                if (curSlot + 3 <= 8)
                {
                    nextSlot = curSlot + 3;
                    rotation = 0;
                }
                break;
            case 2:
                if (curSlot != 0 && curSlot != 3 && curSlot != 6)
                {
                    nextSlot = curSlot - 1;
                    rotation = 90;
                }   
                break;
            case 3:
                if (curSlot != 2 && curSlot != 4 && curSlot != 8)
                {
                    nextSlot = curSlot + 1;
                    rotation = 270;
                }
                break;
        }

        //패킷 보내기
        if (nextSlot != -1)
        {
            GamePacket packet = new()
            {
                DropPlayerSyncRequest = new()
                {
                    SessionId = MinigameManager.Instance.mySessonId,
                    Slot = nextSlot,
                    Rotation = rotation,
                    State = State.Idle,
                }
            };
            SocketManager.Instance.OnSend(packet);
        }

        StunCoroutine ??= StartCoroutine(ButtonCoolTime());
    }

    private void PlayerLeftEvent(int color)
    {
        //UI 가릴거 있으면 여기에 추가. (ex : 등수, 죽은 사람.)
    }

    #region Button Input
    private void EnableBtnInput()
    {
        foreach (Button btn in directionBtns)
        {
            btn.interactable = true;
        }
    }
    private void DisableBtnInput()
    {
        foreach (Button btn in directionBtns)
        {
            btn.interactable = false;
        }
    }
    private IEnumerator ButtonCoolTime()
    {
        DisableBtnInput();
        yield return new WaitForSeconds(0.5f);
        EnableBtnInput();
        StunCoroutine = null;
    }
    #endregion

    #region Deco
    private void DescTxtEffect(bool isFall)
    {
        if (isFall)
        {
            descTxt.text = "떨어집니다!!!";
            descTxt.color = Color.yellow;
            descTxt.rectTransform.localScale = Vector3.one;

            descTxt.rectTransform.DOAnchorPosX(5f, 0.1f).SetRelative().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        else
        {
            descTxt.text = "원하는 위치로 이동하세요~";
            descTxt.color = Color.white;
            descTxt.rectTransform.localScale = Vector3.one;

            descTxt.rectTransform.DOAnchorPosY(10f, 1f).SetRelative().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            descTxt.rectTransform.DOScale(1.2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }
    #endregion
}