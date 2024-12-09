using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderInput : SelectOrderEvent
{
    public void OnAim(InputValue value)
    {
            Vector2 input = value.Get<Vector2>().normalized;
            CallAimEvent(input);
    }

    public void OnShoot(InputValue value)
    {

    }
}
