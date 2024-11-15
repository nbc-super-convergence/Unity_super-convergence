using System.Collections.Generic;
using UnityEngine;

public interface IBoardNode
{
    public Transform transform { get; }
    //다음 노드들 저장
    public List<Transform> nodes { get; }

    bool TryRunNextNode(out Transform node);
}

public interface IAction
{
    //칸에 도착했을떄 작동하는 액션
    void Action();
}

public interface IPurchase : IAction
{
    void Purchase(int index);
}

public interface IToggle : IAction
{
    void Toggle();
}