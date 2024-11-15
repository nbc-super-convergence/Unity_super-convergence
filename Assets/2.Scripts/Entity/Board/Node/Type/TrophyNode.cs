using UnityEngine;

public class TrophyNode : ActionNode
{
    protected override IAction CreateAction()
    {
        return new TrophyArea();
    }
}
