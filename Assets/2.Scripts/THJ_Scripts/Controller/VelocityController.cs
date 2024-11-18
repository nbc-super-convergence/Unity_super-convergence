using UnityEngine;

public class VelocityController : IController
{
    private Rigidbody rgdby;
    private float slideFactor;  // 감속 효과

    public VelocityController(Rigidbody _rgdby, float _factor = 0f)
    {
        rgdby = _rgdby; 
        slideFactor = _factor;
    }

    public void Move(Vector3 vel)
    {
        vel.x *= slideFactor;
        vel.z *= slideFactor;
        rgdby.velocity = vel;    // velocity에 맞게 적용 (임시로)
    }

    public virtual void Interaction(bool isPress)
    {

    }
    public virtual void Jump()
    {

    }
}
