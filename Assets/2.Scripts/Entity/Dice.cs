using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public List<Mesh> numbers;
    private MeshFilter meshFilter;
    private Transform target;
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
        target = t;
    }

    private void Update()
    {
        transform.position = target.position + (Vector3.up * 2.5f);
    }

    public IEnumerator SetDice(int index)
    {
        gameObject.SetActive(true);

        meshFilter.sharedMesh = numbers[index];

        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);  
    }
}
