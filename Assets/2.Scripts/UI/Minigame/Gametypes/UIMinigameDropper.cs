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

    [Header("Effect")]
    [SerializeField] private TextMeshProUGUI descTxt;
    private Light spotLight;

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

        spotLight = MinigameManager.Instance.GetMap<MapGameDropper>()
            .spotLight;
    }

    public override void Closed(object[] param)
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
        descTxt.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator StartCountDown(long startdelay)
    {
        Sequence readySequence = DOTween.Sequence();

        /*"Ready?" 연출*/
        StartCountTxt.gameObject.SetActive(true);
        StartCountTxt.text = "Ready?";
        StartCountTxt.color = new Color(StartCountTxt.color.r, StartCountTxt.color.g, StartCountTxt.color.b, 0);
        readySequence.AppendInterval(0.2f);
        readySequence.Append(StartCountTxt.DOFade(1, 0.5f)); 
        readySequence.AppendInterval(0.8f);   
        readySequence.Append(StartCountTxt.DOFade(0, 0.5f));

        yield return new WaitUntil(() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= startdelay); //시작시간 대기

        gameData.phase++; //바닥 없어짐
        EnableBtnInput(); //input 활성화

        //"Start!" 연출
        Sequence startSequence = DOTween.Sequence();
        startSequence.AppendCallback(() =>
        {
            StartCountTxt.text = "Start!";
            StartCountTxt.color = new Color(StartCountTxt.color.r, StartCountTxt.color.g, StartCountTxt.color.b, 0);
            StartCountTxt.transform.localScale = Vector3.one * 0.8f;
        });
        startSequence.Append(StartCountTxt.DOFade(1, 0.5f));
        startSequence.Join(StartCountTxt.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutQuad));
        startSequence.AppendInterval(0.5f); 
        startSequence.Append(StartCountTxt.DOFade(0, 0.3f));
        startSequence.OnComplete(() =>
        {
            StartCountTxt.gameObject.SetActive(false);
        });
        yield return startSequence.WaitForCompletion();

        descTxt.transform.parent.gameObject.SetActive(true);
        DescEffect(false);
    }

    public IEnumerator MovableTime()
    {
        if (StunCoroutine != null)
        {
            StopCoroutine(StunCoroutine);
            StunCoroutine = null;
        }

        DisableBtnInput();
        DescEffect(true);
        
        yield return new WaitForSeconds(2.5f);
        EnableBtnInput();
        DescEffect(false);
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
    private void DescEffect(bool isFall)
    {
        descTxt.rectTransform.DOKill();

        if (isFall)
        {
            //text dotween
            descTxt.text = "떨어집니다!!!";
            descTxt.color = Color.yellow;
            descTxt.rectTransform.localScale = Vector3.one;
            descTxt.rectTransform.DOAnchorPosX(5f, 0.1f)
                .SetRelative()
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            //light dotween
            spotLight.color = Color.red;
            DOTween.To(
                () => spotLight.intensity,
                x => { spotLight.intensity = x; },
                spotLight.intensity / 2,
                0.5f
                ).SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            //text dotween
            descTxt.text = "It's Dance Time ~";
            descTxt.color = Color.white;
            descTxt.rectTransform.localScale = Vector3.one;
            descTxt.rectTransform.DOScale(1.2f, 1f)
                .SetEase(Ease.InBack)
                .SetLoops(-1, LoopType.Yoyo);

            //light dotween
            spotLight.color = Color.red;
            Color.RGBToHSV(spotLight.color, out float initialHue, out float saturation, out float value);
            DOTween.To(
                () => initialHue,
                x => { spotLight.color = Color.HSVToRGB(x % 1f, saturation, value); },
                1f,//목표 색깔
                3f //애니메이션 시간
            ).SetEase(Ease.Linear) 
             .SetLoops(-1, LoopType.Restart); 
        }
    }
    #endregion
}