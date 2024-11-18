﻿using UnityEngine;

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
