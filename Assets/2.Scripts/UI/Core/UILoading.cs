using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UILoading : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeBG;
    [SerializeField] private Transform rotateImg;

    private readonly float fadeDuration = 0.5f;
    private readonly float rotateSpeed = 200f;

    private Sequence sequence;
    public List<TaskCompletionSource<bool>> taskStates = new();

    private bool isFading = false;
    private Task fadeTask = Task.CompletedTask;

    public async void OnLoadingEvent(TaskCompletionSource<bool> taskState)
    {
        taskStates.Add(taskState);

        if (fadeTask != Task.CompletedTask && !fadeTask.IsCompleted)
        {//fade in 이 진행중일 때.
            await fadeTask;
            isFading = false;
        }

        if (!isFading)
        {//fade out 상태가 아닐 때.
            isFading = true;
            fadeBG.alpha = 1f;
        }

        if (sequence == null)
        {//rotation이 진행중이지 않을 때.
            StartDotweenSequence();
        }
        await taskState.Task;

        taskStates.Remove(taskState);

        if (taskStates.Count == 0)
        {
            fadeTask = FadeIn();
            await fadeTask;
            isFading = false;
        }
    }

    private void StartDotweenSequence()
    {
        rotateImg.gameObject.SetActive(true);

        sequence = DOTween.Sequence();
        sequence.Append(
            rotateImg.DORotate(new Vector3(0f, 360f, 0f), 360f / rotateSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
        );
        sequence.SetLoops(-1, LoopType.Restart);
    }

    private async Task FadeIn()
    {
        if (sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }

        rotateImg.gameObject.SetActive(false);
        await fadeBG.DOFade(0, fadeDuration).AsyncWaitForCompletion();
        fadeBG.blocksRaycasts = false;
    }
}
