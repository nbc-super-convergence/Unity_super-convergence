using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public List<Mesh> numbers;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void ShowDice(int index)
    {
        meshFilter.sharedMesh = numbers[index];
    }
}
