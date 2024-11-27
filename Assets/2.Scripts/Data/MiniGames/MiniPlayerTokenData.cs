using System;
using UnityEngine;

[Serializable]
public class MiniPlayerTokenData
{
    private Animator animator;

    #region common
    public int miniPlayerId;
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

    #region WASD
    public bool WASDInput;
    public Vector2 moveVector;
    #endregion

    #region Ice Datas
    public readonly float icePlayerSpeed = 10f;
    #endregion

    public MiniPlayerTokenData(Animator _anim)
    {
        animator = _anim;
    }
}
