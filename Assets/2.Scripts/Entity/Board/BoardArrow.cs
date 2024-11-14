using System;
using UnityEngine;

public class BoardArrow : MonoBehaviour
{
    [SerializeField] Material normal, onCursor;
    [SerializeField] MeshRenderer meshRenderer;

    private Transform targetNode;
    public event Action OnEvent;

    private void OnEnable()
    {
        meshRenderer.material = normal;
    }

    private void OnMouseEnter()
    {
        meshRenderer.material = onCursor;
    }

    private void OnMouseExit()
    {
        meshRenderer.material = normal;
    }

    private void OnMouseDown()
    {
        OnEvent?.Invoke();

        PlayerTokenHandler p = MapControl.Instance.Curplayer;
        p.SetNode(targetNode,true);
        p.GetDice(0);
    }

    public void SetNode(Transform t)
    {
        targetNode = t;
    }
}
