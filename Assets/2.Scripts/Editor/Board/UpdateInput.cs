using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System;

public class UpdateInput : IInputAction
{
    private BoardCreator b;
    private Stack<Action> stack = new();

    public UpdateInput(BoardCreator b)
    {
        this.b = b;
    }

    public void Arrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (CustomCreate.nodes.Count == 0) return;

            int index = b.indexs[(int)IndexType.NextNode];
            int count = CustomCreate.nodes.Count;
            string input = context.action.activeControl.ToString();

            int num = 0;

            if (input.Contains("right")) num = 1;
            else num = -1;

            b.indexs[(int)IndexType.NextNode] = (index + num + count) % count;
        }
    }

    public void BackSpace(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (stack.Count == 0) return;

            Action action = stack.Pop();
            action?.Invoke();
        }
    }

    public void Enter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int i = b.indexs[(int)IndexType.Prefab];
            int j = b.indexs[(int)IndexType.NextNode];

            BaseNode cur = CustomCreate.nodes[i];
            BaseNode next = CustomCreate.nodes[j];

            cur.nodes.Add(next.transform);
            stack.Push(() => { cur.nodes.Remove(next.transform); });
        }
    }

    public void WASD(InputAction.CallbackContext context)
    {
        int index = b.indexs[(int)IndexType.Prefab];
        int count = CustomCreate.nodes.Count;
        Vector3 input = context.ReadValue<Vector3>();

        int num = 0;

        if (input == Vector3.right) num = 1;
        else if(input == Vector3.left) num = -1;

        b.indexs[(int)IndexType.Prefab] = (index + num + count) % count;
    }

    public void InputEnter()
    {
        b.actions[(int)InputType.WASD] += WASD;
        b.actions[(int)InputType.Arrow] += Arrow;
        b.actions[(int)InputType.BackSpace] += BackSpace;
        b.actions[(int)InputType.Enter] += Enter;
    }

    public void InputExit()
    {
        b.actions[(int)InputType.WASD] -= WASD;
        b.actions[(int)InputType.Arrow] -= Arrow;
        b.actions[(int)InputType.BackSpace] -= BackSpace;
        b.actions[(int)InputType.Enter] -= Enter;
    }

    //public void Draw()
    //{
    //    int i = b.indexs[(int)IndexType.Prefab];

    //    if (c.nodes.Count <= 1) return;

    //    List<Transform> list = c.nodes[i].nodes;
    //    Vector3 pos = c.nodes[i].transform.position;
    //    Handles.color = Color.green;

    //    foreach(Transform t in list)
    //        Handles.DrawAAPolyLine(5f,pos,t.position);
    //}
}
