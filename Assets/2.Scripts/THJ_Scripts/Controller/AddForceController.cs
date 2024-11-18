using UnityEngine;

public class AddForceController : IController
{
    public virtual void Move()
    {

    }
}

//이것들은 앞에거부터 틀을 잡고 구현할 거임
public class VelocityController : IController
{
    public virtual void Move()
    {

    }
}

public class ButtonController : IController
{
    public virtual void Move()
    {

    }
}