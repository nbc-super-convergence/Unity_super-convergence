public interface IAnimation
{
    public void Start();
    public void End();
    public void Update();
}

public abstract class AnimState
{
    protected IAnimation curAnim;

    public void ChangeAnimation(IAnimation curAnim)
    {
        this.curAnim?.End();
        this.curAnim = curAnim;
        this.curAnim?.Start();
    }

    public void Update()
    {
        curAnim?.Update();
    }
}
