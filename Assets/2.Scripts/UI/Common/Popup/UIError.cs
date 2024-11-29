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

        sbInfo.Clear().AppendLine( errorNum.ToString() );
        infoTMP.text = sbInfo.ToString();
    }

    public void OnBtnClose()
    {
        UIManager.Hide<UIError>();
    }   
}