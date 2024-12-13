using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RectTransform image;
    public float rotateSpeed = 200f;

    private void FixedUpdate()
    {
        image.Rotate(0f, (rotateSpeed * Time.deltaTime), 0f);
    }

    public void SetActive(bool boolean)
    {
        gameObject.SetActive(boolean);
    }
}
