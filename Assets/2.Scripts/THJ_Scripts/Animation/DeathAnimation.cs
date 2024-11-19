
using UnityEngine;

public class DeathAnimation : PlayerBaseAnimation
{
    public DeathAnimation(PlayerAnimState animState) : base(animState)
    {
    }

    public override void Start()
    {
        HashCode = Animator.StringToHash("Death");
        base.Start();
    }

    public override void End()
    {
        base.End();
    }
}
