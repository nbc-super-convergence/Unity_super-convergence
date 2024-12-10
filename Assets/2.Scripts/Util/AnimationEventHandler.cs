using System;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnAnimationComplete;

    public void AnimationCompleted()
    {
        OnAnimationComplete?.Invoke();
    }
}