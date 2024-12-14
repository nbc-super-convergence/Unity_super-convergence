using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCanvas : MonoBehaviour
{
    [SerializeField] private List<Transform> parents = new();

    private async void Start()
    {
        UIManager.SetParents(parents);
        await UIManager.Show<BoardUI>();

        UIManager.SceneChangeTask.SetResult(true);
    }
}
