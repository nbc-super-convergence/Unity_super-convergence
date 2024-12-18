using DG.Tweening;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    private Sequence sequence;

    private readonly float rotateSpeed = 100f;

    private void Start()
    {
        StartDotweenSequence();
    }

    private void StartDotweenSequence()
    {
        sequence = DOTween.Sequence();
        sequence.Append(
            transform.DORotate(new Vector3(0f, 360f, 0f), 360f / rotateSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
        );
        sequence.SetLoops(-1, LoopType.Restart);
    }

}
