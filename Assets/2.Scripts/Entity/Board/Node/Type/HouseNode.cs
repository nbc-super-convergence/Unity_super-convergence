using UnityEngine;

public class HouseNode : ActionNode
{
    [SerializeField] MeshRenderer plane;

    protected override IAction CreateAction()
    {
        return new ActionArea(plane);
    }
}
