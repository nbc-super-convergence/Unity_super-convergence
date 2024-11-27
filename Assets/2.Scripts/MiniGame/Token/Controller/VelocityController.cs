using UnityEngine;

public class VelocityController : MiniPlayerController
{
    public VelocityController(MiniPlayerTokenData playerData, Rigidbody rb) : base(playerData, rb)
    {
    }

    public void Move(Vector3 vel)
    {
        rb.velocity = vel;
    }
}
