using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[CustomEditor(typeof(BoardCreator))]
public class CustomCreate : Editor
{
    List<int> indexs;
    string[] tools = { "Create", "Connect","Move" };
    const string path = "Assets/AddressableDatas/Prefab/addressableMap.json";
    public List<GameObject> prefabs = new();
    public static List<BaseNode> nodes = new();
    public List<Transform> selects = new();

    private BoardCreator b;

    List<IInputAction> inputs = new List<IInputAction>();

    private void Awake()
    {
        b = (BoardCreator)target;

        b.actions[(int)InputType.Tab] = Tab;

        inputs.Add(new CreateInput(b, this));
        inputs.Add(new ConnectInput(b));
        inputs.Add(new MoveInput(b, this));

        GameObject g = GameObject.Find("Board");

        if(g != null)
            nodes = g.GetComponentsInChildren<BaseNode>().ToList();
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;

        int index = indexs[(int)IndexType.Tool];
        indexs[(int)IndexType.Tool] = GUILayout.Toolbar(index, tools);

        switch (index)
        {
            case 0:
                DrawGrid();
                break;
            case 1:

                break;
            case 2:

                break;
        }
    }

    private void OnSceneGUI()
    {
        if (!Application.isPlaying) return;

        int index = indexs[(int)IndexType.Tool];

        switch (index)
        {
            case 0:
                //DrawGrid();
                break;
            case 1:
                DrawLine();
                break;
            case 2:
                DrawSelect();
                break;
            case 3:

                break;
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying) return;

        LoadPrefab();
        indexs = b.indexs;

        int index = indexs[(int)IndexType.Tool];
        inputs[index].InputEnter();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying) return;

        int index = indexs[(int)IndexType.Tool];
        inputs[index].InputExit();
    }

    private void Tab(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int index = indexs[(int)IndexType.Tool];
            inputs[index].InputExit();

            index = (indexs[(int)IndexType.Tool] + 1) % inputs.Count;
            indexs[(int)IndexType.Tool] = index;
            inputs[index].InputEnter();
        }
    }

    #region 생성
    private async void LoadPrefab()
    {
        var text = await Addressables.LoadAssetAsync<TextAsset>(path).Task;

        AddressableMapList mapList = JsonUtility.FromJson<AddressableMapList>(text.text);

        foreach (var l in mapList.list)
        {
            string p = l.path;

            if (!l.path.Contains("Nodes")) continue;

            var g = await Addressables.LoadAssetAsync<GameObject>(l.path).Task;

            prefabs.Add(g);
        }
    }

    void DrawGrid()
    {
        GUILayout.Label("Node Grid");

        Texture[] textures = new Texture[prefabs.Count];

        for (int i = 0; i < prefabs.Count; i++)
            textures[i] = AssetPreview.GetAssetPreview(prefabs[i]);

        int index = indexs[(int)IndexType.Grid];
        indexs[(int)IndexType.Grid] = GUILayout.SelectionGrid(index, textures, 4, GUILayout.MaxWidth(300), GUILayout.MinHeight(100));

        //if(gridIndex != b.gridIndex)
        //    b.gridIndex = gridIndex;
    }

    //public void SetGridIndex(int i)
    //{
    //    int c = prefabs.Count;

    //    if (i == 0) return;

    //    int index = indexs[(int)IndexType.Grid];
    //    indexs[(int)IndexType.Grid] = index = (index + i + c) % c;

    //    b.SetMesh(prefabs[index].GetComponent<MeshFilter>().sharedMesh);

    //    //b.gridIndex = gridIndex;
    //}

    //private void Create()
    //{
    //    int index = indexs[(int)IndexType.Grid];
    //    GameObject g = Instantiate(prefabs[index],b.transform.position,Quaternion.identity);
    //    var c = g.GetComponent<BaseNode>();

    //    nodes.Add(c);
    //    st.Push(g);
    //}

    //private void Remove()
    //{
    //    if (st.Count == 0) return;

    //    var g = st.Pop();
    //    Destroy(g);
    //}

    #endregion

    #region 연결

    private void DrawLine()
    {
        if (nodes.Count == 0) return;

        int i = b.indexs[(int)IndexType.Prefab];
        int j = b.indexs[(int)IndexType.NextNode];

        List<Transform> list = nodes[i].nodes;
        Transform t = nodes[j].transform;

        Vector3 p = nodes[i].transform.position;

        Handles.color = Color.green;
        Handles.DrawSolidDisc(p, Vector3.up, 1f);

        foreach (Transform transform in list)
            Handles.DrawAAPolyLine(5f, p, transform.position);

        if (i == j || list.Contains(t)) return;

        Handles.color = Color.red;
        Handles.DrawAAPolyLine(5f,p,t.position);
    }



    //private void Show()
    //{
    //    BaseNode node = nodes[nodeIndex];
    //    Handles.color = Color.green;
    //    foreach(var n in node.nodes)
    //        Handles.DrawAAPolyLine(5f,node.transform.position,n.transform.position);

    //    BaseNode n1 = nodes[nodeIndex];
    //    BaseNode n2 = nodes[nextNodeIndex];

    //    Handles.color = Color.red;
    //    if (n1 != n2 && !n1.nodes.Contains(n2.transform)) 
    //        Handles.DrawAAPolyLine(5f, n1.transform.position, n2.transform.position);

    //}

    //private void ChangeNode(int i)
    //{
    //    int c = nodes.Count;
    //    nextNodeIndex = (nextNodeIndex + i + c) % c;
    //}

    //private void SetNode()
    //{
    //    BaseNode n1 = nodes[nodeIndex];
    //    BaseNode n2 = nodes[nextNodeIndex];

    //    if (n1 == n2 || n1.nodes.Contains(n2.transform)) return;

    //    n1.nodes.Add(n2.transform);
    //}

    #endregion

    #region 이동

    void DrawSelect()
    {
        List<GameObject> list=  new List<GameObject>();

        foreach(var t in selects)
            list.Add(t.gameObject);

        Handles.DrawOutline(list,Color.green);

        Handles.color = Color.red;
        int index = indexs[(int)IndexType.Prefab];
        Transform cur = nodes[index].transform;
        Handles.DrawSolidDisc(cur.position, Vector3.up, .5f);
    }

    #endregion

    #region 정렬



    #endregion
}
#endif