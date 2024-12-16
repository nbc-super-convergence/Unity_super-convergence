using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(BaseNode),true)]
public class ViewNextNode : Editor
{
    BaseNode b;

    private void Awake()
    {
        b = (BaseNode)target;
    }

    private void OnSceneGUI()
    {
        Vector3 p = b.transform.position;
        var list = b.nodes;
        Handles.color = Color.green;

        foreach (Transform transform in list)
            Handles.DrawAAPolyLine(5f, p, transform.position);

        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(5f,p,b.lineUp.position);
    }
}
#endif