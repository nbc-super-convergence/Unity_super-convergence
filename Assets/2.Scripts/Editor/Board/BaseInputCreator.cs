
using UnityEngine.InputSystem;


public interface IInputAction
{
    public void InputEnter();
    public void InputExit();
    public void WASD(InputAction.CallbackContext context);
    public void Enter(InputAction.CallbackContext context);
    public void Arrow(InputAction.CallbackContext context);
    public void BackSpace(InputAction.CallbackContext context);
}
