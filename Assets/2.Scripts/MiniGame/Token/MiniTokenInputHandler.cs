using UnityEngine;
using UnityEngine.InputSystem;

public class MiniTokenInputHandler 
{
    private PlayerInput playerInput;
    private MiniTokenData playerData; 

    public MiniTokenInputHandler(MiniTokenData data)
    {
        playerInput = new PlayerInput();
        playerData = data;
    }

    public void EnablePlayerInput()
    {
        playerInput.Enable();

        playerInput.MiniPlayerToken.Move.performed += OnMove;
        playerInput.MiniPlayerToken.Move.canceled += OnMoveCanceled;
    }

    public void DisablePlayerInput()
    {
        playerInput.Disable();

        playerInput.MiniPlayerToken.Move.performed -= OnMove;
        playerInput.MiniPlayerToken.Move.canceled -= OnMoveCanceled;
    }

    #region WASD 이동
    private void OnMove(InputAction.CallbackContext context)
    {
        playerData.wasdVector = context.ReadValue<Vector2>();
        playerData.rotY = Mathf.Atan2(playerData.wasdVector.x, playerData.wasdVector.y) * Mathf.Rad2Deg;
        playerData.CurState = State.Move;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        playerData.wasdVector = Vector2.zero;
        playerData.CurState = State.Idle;
    }
    #endregion

    private void OnJump(InputAction.CallbackContext context)
    {
        //float pressAnalog = 0f; //키를 어느정도 누르고 있는지
        //if (context.phase == InputActionPhase.Performed)
        //{
        //    //점프
        //    pressAnalog += Time.deltaTime;
        //}
        //else if (context.phase == InputActionPhase.Canceled)
        //{
        //    pressAnalog = 0f;
        //}
        //anim state -> jump
    }
}
