using UnityEngine;

public class ButtonController : IController
{
    public virtual void Move(Vector3 pos)
    {

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