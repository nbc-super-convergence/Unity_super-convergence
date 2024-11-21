using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class CreateInput : IInputAction
{
    private BoardCreator b;
    private CustomCreate c;
    private Stack<GameObject> stack;
    private List<BaseNode> nodes;

    public CreateInput(BoardCreator b, CustomCreate c)
    {
        this.b = b; 
        this.c = c;

        stack = c.stack;
        nodes = c.nodes;
    }

    public void Arrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int index = b.indexs[(int)IndexType.Grid];
            int count = c.prefabs.Count;
            string input = context.action.activeControl.ToString();

            int num = 0;

            if (input.Contains("right")) num = 1;
            else num = -1;

            b.indexs[(int)IndexType.Grid] = (index + num + count) % count;
        }
    }

    public void BackSpace(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (stack.Count == 0) return;

            var g = stack.Pop();
            Object.Destroy(g);
            nodes.Remove(g.GetComponent<BaseNode>());
        }
    }

    public void Enter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int index = b.indexs[(int)IndexType.Grid];

            var g = c.prefabs[index];
            var o = Object.Instantiate(g, b.transform.position, Quaternion.identity);
            stack.Push(o);
            nodes.Add(o.GetComponent<BaseNode>());
        }
    }

    public void WASD(InputAction.CallbackContext context)
    {
        b.dir = context.ReadValue<Vector3>();
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
        b.dir = Vector3.zero;

        b.actions[(int)InputType.WASD] -= WASD;
        b.actions[(int)InputType.Arrow] -= Arrow;
        b.actions[(int)InputType.BackSpace] -= BackSpace;
        b.actions[(int)InputType.Enter] -= Enter;
    }
}