
public class PlayerBaseAnimation : IAnimation
{
    protected PlayerAnimState animState;

    public int HashCode { get; protected set; }

    public PlayerBaseAnimation(PlayerAnimState animState)
    {
        this.animState = animState;
    }

    public virtual void Start()
    {
        AnimationStart(HashCode);
    }

    public virtual void End()
    {
        AnimationEnd(HashCode);
    }

    public virtual void Update()
    {

    }

    protected void AnimationStart(int hashCode)
    {
        //animState.Player.animator.SetBool(hashCode, true);
    }

    protected void AnimationEnd(int hashCode)
    {
        //animState.Player.animator.SetBool(hashCode, false);
    }
}
