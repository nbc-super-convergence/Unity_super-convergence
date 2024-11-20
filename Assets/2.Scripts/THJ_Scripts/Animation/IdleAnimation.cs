
using UnityEngine;

public class IdleAnimation : PlayerBaseAnimation
{
    public IdleAnimation(PlayerAnimState animState) : base(animState)
    {
    }

    public override void Start()
    {
        HashCode = Animator.StringToHash("Idle");
        base.Start();
    }

    public override void End()
    {
        base.End();
    }
}
