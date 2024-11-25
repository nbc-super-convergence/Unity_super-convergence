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
    void Purchase(int index);
}

public interface IToggle
{
    void Toggle();
}