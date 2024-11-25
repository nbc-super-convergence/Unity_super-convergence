using System.Collections.Generic;
using UnityEngine;

public interface IBoardNode
{
    public Transform transform { get; }

    bool TryGetNode(out Transform node);
}

public interface IAction
{
    //ĭ�� ���������� �۵��ϴ� �׼�
    void Action();
}

public interface IPurchase
{
    string message { get; }
    void Purchase(int index);
    void Cancle();
}

public interface IToggle
{
    void Toggle();
}