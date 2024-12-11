using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderEvent : MonoBehaviour
{
    public event Action<Vector2> OnAimEvent;
    public event Action<bool> OnShootEvent;

    public void CallAimEvent(Vector2 direction)
    {
        OnAimEvent?.Invoke(direction);
    }
    public void CallShootEvent(bool press)
    {
        OnShootEvent?.Invoke(press);
    }

    public void OnAim(InputValue value)
    {
        Vector2 input = value.Get<Vector2>().normalized;
        CallAimEvent(input);
    }
    public void OnShoot(InputValue value)
    {
        CallShootEvent(value.isPressed);
    }
}
