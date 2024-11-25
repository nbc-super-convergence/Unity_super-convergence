
using UnityEngine;

public class RunAnimation : PlayerBaseAnimation
{
    public RunAnimation(PlayerAnimState animState) : base(animState)
    {
    }

    public override void Start()
    {
        HashCode = Animator.StringToHash("Run");
        base.Start();
    }

    public override void End()
    {
        base.End();
    }
}
