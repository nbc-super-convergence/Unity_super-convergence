using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public struct PrevMove
{
    public Transform transform;
    public Vector3 positon;

    public PrevMove(Transform transform, Vector3 positon)
    {
        this.transform = transform;
        this.positon = positon;
    }
}


public class MoveInput : IInputAction
{
    BoardCreator b;
    CustomCreate c;
    List<Transform> selects;
    Stack<List<PrevMove>> stack = new();

    int prevIndex = 0;
    bool isSelect = false;
    private bool isperformed = false;
    private int performCount;

    public MoveInput(BoardCreator b,CustomCreate c)
    {
        this.b = b;
        this.c = c;

        selects = c.selects;
    }

    public void Arrow(InputAction.CallbackContext context)
    {
        if (CustomCreate.nodes.Count == 0) return;

        string input = context.action.activeControl.ToString();

        int num = 0;

        if (input.Contains("right")) num = 1;
        else num = -1;

        if (context.performed)
        {
            isperformed = true;
            b.StartCoroutine(Arrow(num));
        }
        else if (context.canceled)
            isperformed = false;
    }

    public IEnumerator Arrow(int num)
    {
        float time = 1f;
        while (isperformed)
        {
            time += Time.deltaTime;

            if (time > 0.2f)
            {
                int index = b.indexs[(int)IndexType.Prefab];
                int count = CustomCreate.nodes.Count;

                index = b.indexs[(int)IndexType.Prefab] = (index + num + count) % count;
                var list = CustomCreate.nodes;

                if (isSelect && !selects.Contains(list[index].transform))
                    selects.Add(list[index].transform);

                time = 0.0f;
            }

            yield return null;
        }
    }

    public void BackSpace(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (stack.Count == 0) return;

            List<PrevMove> list = stack.Pop();
            BackSpace(list);
        }
    }

    public void Enter(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isSelect = !isSelect;

            if (isSelect)
            {
                int index = b.indexs[(int)IndexType.Prefab];

                selects.Add(CustomCreate.nodes[index].transform);
                prevIndex = index;
            }
            else
                selects.Clear();
        }
    }

    public void WASD(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Vector3 dir = context.ReadValue<Vector3>() * 0.5f;

            performCount = (performCount + 1) % 2;

            if (isSelect && performCount == 1)
            {
                List<PrevMove> list = new List<PrevMove>();

                foreach (var g in selects)
                {
                    PrevMove prev = new PrevMove(g,g.position);
                    list.Add(prev);
                }

                stack.Push(list);

                foreach (var g in selects)
                    g.position += dir;

                b.StartCoroutine(WASD(dir));
            }
        }
    }

    public IEnumerator WASD(Vector3 dir)
    {
        float time = 1f;

        while (performCount == 1)
        {
            time += Time.deltaTime;

            if (time > 0.2f)
            {
                foreach (var g in selects)
                    g.position += dir;

                time = 0.0f;
            }

            yield return null;
        }
    }

    private void BackSpace(List<PrevMove> list)
    {
        for (int i = 0; i < list.Count; i++)
            list[i].transform.position = list[i].positon;
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

        b.indexs[(int)IndexType.Prefab] = prevIndex;
        selects.Clear();
    }
}
