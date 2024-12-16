using System.Collections.Generic;
using UnityEngine;

public interface IBoardNode
{
    public Transform transform { get; }

    bool TryGetNode(out Transform node);

    public List<Transform> GetList();
    public void LineUp();
}

public interface IAction
{
    //칸에 도착했을떄 작동하는 액션
    void Action();
}

public interface IPurchase
{
    string message { get; }
    void Purchase();
    void Cancle();
}

public interface IToggle
{
    void Toggle();
}