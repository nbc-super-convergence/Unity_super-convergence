public abstract class ActionNode : BaseNode,IAction
{
    IAction action;

    private void Awake()
    {
        action = CreateAction();
    }

    public void Action()
    {
        StartCoroutine(ArrivePlayer(action.Action));
    }

    protected abstract IAction CreateAction();
}
