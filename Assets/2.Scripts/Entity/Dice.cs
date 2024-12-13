using System.Collections.Generic;
using System.Collections;
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

    public void SetDicePosition(Transform t)
    {
        transform.position = t.position + (Vector3.up * 2.5f);
    }

    public IEnumerator SetDice(int index)
    {
        meshFilter.sharedMesh = numbers[index];

        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);  
    }
}
