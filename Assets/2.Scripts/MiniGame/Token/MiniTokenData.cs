using System;
using UnityEngine;

[Serializable]
public class MiniTokenData
{
    private readonly Animator animator;

    #region common : 모든 게임에서 사용
    public int tokenColor;
    private State curState;
    public State CurState
    {
        get => curState;
        set
        {
            if (curState != value)
            {
                curState = value;
                AnimState.ChangePlayerAnimState(animator, curState);
            }
        }
    }
    #endregion

    #region Inputs
    public Vector2 wasdVector;
    public Vector3 nextPos;
    public float rotY = 0f;
    public float PlayerSpeed = 15f;

    public int arrowInput;
    #endregion

    public MiniTokenData(Animator _anim, int color)
    {
        animator = _anim;
        tokenColor = color;
        curState = State.Idle;
    }
}
