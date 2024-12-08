using System;
using UnityEngine;
public static class AnimState
{
    #region Animation Hashes
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private static readonly int DanceUpHash = Animator.StringToHash("DanceUp");
    private static readonly int DanceDownHash = Animator.StringToHash("DanceDown");
    private static readonly int DanceLeftHash = Animator.StringToHash("DanceLeft");
    private static readonly int DanceRightHash = Animator.StringToHash("DanceRight");
    private static readonly int DanceSlipHash = Animator.StringToHash("DanceSlip");
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

            // 서버처리가 먼저필요?  // Protocol.cs에 임시로 직접 입력함.
            case State.DanceUp:
                player.SetTrigger(DanceUpHash);
                break;
            case State.DanceDown:
                player.SetTrigger(DanceDownHash);
                break;
            case State.DanceLeft:
                player.SetTrigger(DanceLeftHash);
                break;
            case State.DanceRight:
                player.SetTrigger(DanceRightHash);
                break;
            case State.DanceSlip:
                player.SetTrigger(DanceSlipHash);
                break;
        }
    }
}
