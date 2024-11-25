using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BoardManager))]
public class BoardManagerTest : Editor
{
    BoardManager target;

    private void Awake()
    {
        if(Application.isPlaying)
            target = BoardManager.Instance;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;

        if(GUILayout.Button("Dice"))
            target.TestRandomDice();

    }
}
