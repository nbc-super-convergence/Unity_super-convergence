using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverlayClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UIKick uiKick;
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Hide<UIKick>();
    }
}
