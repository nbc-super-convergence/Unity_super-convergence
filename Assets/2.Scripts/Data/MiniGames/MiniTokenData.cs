using System;
using UnityEngine;

[Serializable]
public class MiniTokenData
{
    private readonly Animator animator;

    #region common : 모든 게임에서 사용
    public int miniTokenId;
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
    #endregion

    #region Ice Datas
    public readonly float icePlayerSpeed = 10f;
    #endregion

    public MiniTokenData(Animator _anim)
    {
        animator = _anim;
        curState = State.Idle;
    }
}
