using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum IndexType
{
    Tool,
    Grid,
    Prefab,
    NextNode,
}

public enum InputType
{
    WASD,
    Arrow,
    Enter,
    BackSpace,
    Tab,
}

public class BoardCreator : MonoBehaviour
{
    private MeshFilter filter;

    //IndexType
    public List<int> indexs = new List<int>();

    //InputType
    public readonly Action<InputAction.CallbackContext>[] actions = new Action<InputAction.CallbackContext>[Enum.GetNames(typeof(InputType)).Length];
    public Vector3 dir = Vector3.zero;

    private void Awake()
    {
        Initalize();
        filter = GetComponent<MeshFilter>();
    }

    private void Initalize()
    {
        int index = Enum.GetNames(typeof(IndexType)).Length;

        for (int i = 0; i < index; i++)
            indexs.Add(0);
    }

    private void Update()
    {
        transform.position += dir;
    }

    public void SetMesh(Mesh mesh)
    {
        filter.mesh = mesh;
    }

    #region Inputs
    public void Tab(InputAction.CallbackContext context)
    {
        actions[(int)InputType.Tab]?.Invoke(context);
    }

    public void WASD(InputAction.CallbackContext context)
    {
        actions[(int)InputType.WASD]?.Invoke(context);
    }

    public void Enter(InputAction.CallbackContext context)
    {
        actions[(int)InputType.Enter]?.Invoke(context);
    }

    public void Arrow(InputAction.CallbackContext context)
    {
        actions[(int)InputType.Arrow]?.Invoke(context);
    }

    public void BackSpace(InputAction.CallbackContext context)
    {
        actions[(int)InputType.BackSpace]?.Invoke(context);
    }
    #endregion

    #region Old
    //WASD
    //public void Move(InputAction.CallbackContext context)
    //{
    //    dir = context.ReadValue<Vector3>() * distance;
    //}

    ////Arrow
    //public void ChangeGrid(InputAction.CallbackContext context)
    //{
    //    if(context.started)
    //    {
    //        string input = context.action.activeControl.ToString();
    //        int num = 0;

    //        if (input.Contains("right")) num = 1;
    //        else num = -1;
    //        OnGrid?.Invoke(num);
    //    }
    //}

    ////public void SetDistance(InputAction.CallbackContext context)
    ////{
    ////    if(context.started)
    ////    {
    ////        string input = context.action.activeControl.ToString();

    ////        if (input.Contains("+")) distance += 1;
    ////        else distance -= 1;
    ////    }

    ////    distance = Mathf.Max(distance,1);
    ////}

    ////Enter
    //public void Create(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        OnCreate?.Invoke();
    //    }
    //}

    ////BackSpace
    //public void Remove(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        OnRemove?.Invoke();
    //    }
    //}
    #endregion
}
