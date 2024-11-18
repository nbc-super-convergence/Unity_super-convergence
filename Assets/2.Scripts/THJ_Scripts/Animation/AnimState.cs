using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimState
{
    public abstract void AnimStart();
}


public class Idle : AnimState
{
    public override void AnimStart()
    {

    }
}

public class Move : AnimState
{
    public override void AnimStart()
    {

    }

}

public class Jump : AnimState
{
    public override void AnimStart()
    {

    }

}