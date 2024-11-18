using UnityEngine;

public class AddForceController : IController
{
    private Rigidbody rgdby;
    private Vector3 curPos = Vector3.zero;

    public virtual void Move(Vector3 pos)
    {
        curPos.x = pos.x;
        curPos.z = pos.z;
        rgdby.AddForce(curPos, ForceMode.Impulse);  //이건 물리결과를 보고
    }

    public virtual void Interaction()
    {

    }

    public virtual void Jump()
    {

    }
}

//이것들은 앞에거부터 틀을 잡고 구현할 거임
public class VelocityController : IController
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