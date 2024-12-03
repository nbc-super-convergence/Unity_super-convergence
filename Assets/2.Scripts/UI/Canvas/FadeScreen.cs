using System;
using System.Data.SqlTypes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class FadeScreen : Singleton<FadeScreen>
{
    [SerializeField] private Canvas FadeCanvas;
    [SerializeField] private Image FadeImage;
    public Sequence Sequence;


    public void FadeOut(Action callback = null, float fadeTime = 1f)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.Kill();
            Instance.Sequence.onComplete = null;
        }
        FadeImage.gameObject.SetActive(true);
        Instance.Sequence = DOTween.Sequence().Append(Instance.FadeImage.DOFade(1f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            FadeImage.gameObject.SetActive(false);
            //SoundManager.StopAllEffectSound();
            callback?.Invoke();
        };
    }

    public void FadeIn(Action callback = null, float fadeTime = 1f)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.onComplete = null;
        }

        FadeImage.gameObject.SetActive(true);
        Instance.Sequence = DOTween.Sequence().Append(Instance.FadeImage.DOFade(0f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            FadeImage.gameObject.SetActive(false);
            callback?.Invoke();
        };
    }

}
