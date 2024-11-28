using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationDice : MonoBehaviour
{
    float x, y, z;

    private void OnEnable()
    {
        y = Random.Range(0.5f, 3);
        z = Random.Range(0.5f, 3);
    }

    // Update is called once per frame
    void Update()
    {
         transform.eulerAngles -= new Vector3(0, y, z);
    }
}
