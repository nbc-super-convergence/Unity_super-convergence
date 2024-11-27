using System;
using UnityEngine;
public static class AnimState
{
    #region Animation Hashes
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DieHash = Animator.StringToHash("Die");
    #endregion

    public static void ChangePlayerAnimState(Animator player, State state)
    {
        switch(state)
        {
            case State.Idle:
                player.SetTrigger(IdleHash);
                break;
            case State.Move:
                player.SetTrigger(MoveHash);
                break;
            case State.Die:
                player.SetTrigger(DieHash);
                break;
        }
    }
}
