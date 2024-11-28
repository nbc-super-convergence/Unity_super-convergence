using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ConnectInput : IInputAction
{
    private BoardCreator b;
    private Stack<Action> stack = new();
    private bool isperformed = false;
    private int performCount;

    public ConnectInput(BoardCreator b)
    {
        this.b = b;
    }

    public void Arrow(InputAction.CallbackContext context)
    {
        if (CustomCreate.nodes.Count == 0) return;

        string input = context.action.activeControl.ToString();

        int num = 0;

        if (input.Contains("right")) num = 1;
        else num = -1;

        if (context.started)
        {
            int index = b.indexs[(int)IndexType.NextNode];
            int count = CustomCreate.nodes.Count;

            b.indexs[(int)IndexType.NextNode] = (index + num + count) % count;
        }

        if(context.performed)
        {
            isperformed = true;
            b.StartCoroutine(Arrow(num));
        }
        else if(context.canceled)
            isperformed = false;
    }

    public IEnumerator Arrow(int num)
    {
        float time = 0.0f;
        while(isperformed)
        {
            time += Time.deltaTime;

            if(time > 0.2f)
            {
                int index = b.indexs[(int)IndexType.NextNode];
                int count = CustomCreate.nodes.Count;

                b.indexs[(int)IndexType.NextNode] = (index + num + count) % count;
                time = 0.0f;
            }

            yield return null;
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
        if(context.performed)
        {
            Vector3 input = context.ReadValue<Vector3>();
            int num = 0;

            if (input == Vector3.right) num = 1;
            else if (input == Vector3.left) num = -1;

            performCount = (performCount + 1) % 2;

            if (performCount == 1)
            {
                int index = b.indexs[(int)IndexType.Prefab];
                int count = CustomCreate.nodes.Count;

                b.indexs[(int)IndexType.Prefab] = (index + num + count) % count;

                b.StartCoroutine(WASD(num));
            }
        }
    }

    public IEnumerator WASD(int num)
    {
        float time = 0.0f;

        while (performCount == 1)
        {
            time += Time.deltaTime;

            if (time > 0.2f)
            {
                int index = b.indexs[(int)IndexType.Prefab];
                int count = CustomCreate.nodes.Count;

                b.indexs[(int)IndexType.Prefab] = (index + num + count) % count;
                time = 0.0f;
            }

            yield return null;
        }
    }
    public void InputEnter()
    {
        performCount = 0;

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
