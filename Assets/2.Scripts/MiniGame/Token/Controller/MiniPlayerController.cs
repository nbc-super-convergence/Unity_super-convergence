using UnityEngine;

public abstract class MiniPlayerController
{
    protected MiniPlayerTokenData playerData;
    protected Rigidbody rb;

    public MiniPlayerController(MiniPlayerTokenData playerData, Rigidbody rb)
    {
        this.rb = rb;
        this.playerData = playerData;
    }

    public virtual void MoveVector2() { }
}
