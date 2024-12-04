using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIError : UIBase
{
    [SerializeField] private TMP_Text titleTMP;
    [SerializeField] private TMP_Text infoTMP;
    [SerializeField] private TMP_Text closeCountTMP;
    [SerializeField] private GameObject Count;

    [SerializeField] private int waitSeconds;

    private StringBuilder sbTitle = new();
    private StringBuilder sbInfo = new();

    private Task countdownTask;
    private CancellationTokenSource countdownCts;

    public override void Opened(object[] param)
    {
        SetInfo((GlobalFailCode)param[0]);
        countdownCts = new();
        countdownTask = CountDownAsync(waitSeconds, countdownCts.Token);
    }

    public override void Closed(object[] param)
    {
        if (countdownCts != null && !countdownCts.IsCancellationRequested)
        {
            countdownCts.Cancel();
            countdownCts.Dispose();
            countdownCts = null;
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { OnBtnClose(); }
    }

    private async Task CountDownAsync(int countTime, CancellationToken token)
    {
        if( countTime < 1)
        {
            Count.SetActive(false);
            return;
        }
        try
        {
            while (countTime > 0)
            {
                token.ThrowIfCancellationRequested();
                closeCountTMP.text = countTime--.ToString();
                await Task.Delay(1000);
            }
            OnBtnClose();
        }
        catch (OperationCanceledException)
        {
        }        
    }

    public void SetInfo(GlobalFailCode errorNum, string titleText = null)
    {
        sbTitle.Clear().Append(titleText != null ? titleText : "");
        titleTMP.text = sbTitle.ToString();

        //sbInfo.Clear().AppendLine( ((GlobalFailCodeKorean)(int)errorNum).ToString() );
        sbInfo.Clear().AppendLine(GameManager.Instance.failCodeDic[(int)errorNum]);

        infoTMP.text = sbInfo.ToString();
    }

    public void OnBtnClose()
    {
        UIManager.Hide<UIError>();
    }   
}

// TODO:: CSV와 어드레서블로 스크립트 바깥에서 관리하기.
enum GlobalFailCodeKorean
{
    없음 = 0,
    알_수_없는_오류 = 1,
    잘못된_요청 = 2,
    인증_실패 = 3,

    // 100 ~ 199 인증 서버 오류
    아이디_또는_비밀번호_누락 = 100,
    비밀번호_확인_불일치 = 101,
    이미_존재하는_아이디 = 102,
    이미_로그인된_아이디 = 103,
    유효성_검사_오류 = 104,
    이미_존재하는_닉네임 = 105,

    // 200 ~ 299 로비 서버 오류
    사용자_찾을_수_없음 = 200,
    이미_로비에_있음 = 201,
    사용자가_로비에_없음 = 202,
    로비_사용자_목록_오류 = 203,
    잘못된_로비 = 204,

    // 300 ~ 399 방 서버 오류
    방을_찾을_수_없음 = 300,
    사용자가_이미_방에_있음 = 301,
    사용자가_방에_없음 = 302,
    잘못된_방_상태 = 303,
    방장은_준비할_수_없음 = 304,
    방이_가득_참 = 305,

    // 400 ~ 499 보드 서버 오류

    // 500 ~ 599 ICE 서버 오류
    게임을_찾을_수_없음 = 500,
    게임에서_사용자를_찾을_수_없음 = 501,
}