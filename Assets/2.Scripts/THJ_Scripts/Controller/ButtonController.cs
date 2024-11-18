using UnityEngine;

public class ButtonController : IController
{
    public virtual void Move(Vector3 pos)
    {

    }

    public virtual void Interaction(bool isPress)
    {
        if(isPress)
        {

        }
    }

    public virtual void Jump()
    {

    }
}