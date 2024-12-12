using System.Threading;
using System;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class UICountdown : UIBase
{
    [SerializeField] private TMP_Text startCountdownTMP;

    private int countTime;
    private long startTime;
    private Task countdownTask;
    private CancellationTokenSource countdownCts;
    private Action callback;
    public override void Opened(object[] param)
    {
        if (param[0] is long startTime)
        {
            this.startTime = startTime;
        }
        if (param[1] is int time)
        {
            countTime = time;
        }
        if (param[2] is Action action)
        {
            callback = action;
        }
        else
        {
            callback = null;
        }
        countdownCts = new();
        StartCoroutine(StartCountDown(this.startTime));
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
    private IEnumerator StartCountDown(long unixTime)
    {
        yield return new WaitUntil(() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= unixTime);

        countdownTask = CountDownAsync(countTime, countdownCts.Token, callback);
    }

    private async Task CountDownAsync(int countTime, CancellationToken token, Action callback = null)
    {
        if (countTime < 1)
        {
            startCountdownTMP.gameObject.SetActive(false);
            return;
        }
        try
        {
            while (countTime > 0)
            {
                token.ThrowIfCancellationRequested();
                startCountdownTMP.text = countTime--.ToString();
                await Task.Delay(1000);
            }
            callback?.Invoke();
            startCountdownTMP.text = "시작!";
            await Task.Delay(1000);
            // 준비땅 효과음
            UIManager.Hide<UICountdown>();
        }
        catch (OperationCanceledException)
        {
        }
    }
}