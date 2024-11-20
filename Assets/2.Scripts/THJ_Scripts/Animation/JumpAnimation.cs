
using UnityEngine;

public class JumpAnimation : PlayerBaseAnimation
{
    public JumpAnimation(PlayerAnimState animState) : base(animState)
    {
    }

    public override void Start()
    {
        HashCode = Animator.StringToHash("Jump");
        base.Start();
    }

    public override void End()
    {
        base.End();
    }
}