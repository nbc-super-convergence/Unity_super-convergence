using UnityEngine;

public class VelocityController : IController
{
    private Rigidbody rgdby;
    private Vector3 curPos = Vector3.zero;

    public virtual void Move(Vector3 pos)
    {
        curPos.x = pos.x;
        curPos.z = pos.z;
        rgdby.velocity = curPos;    // velocity에 맞게 적용 (임시로)
    }

    public virtual void Interaction()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Jump()
    {
        throw new System.NotImplementedException();
    }
}
