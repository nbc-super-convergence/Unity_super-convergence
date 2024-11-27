using UnityEngine;
using UnityEngine.InputSystem;

public class MiniPlayerInputHandler 
{
    private PlayerInput playerInput;
    private MiniPlayerTokenData playerData; 

    public void Init(MiniPlayerTokenData data)
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

    #region WASD �̵�
    private void OnMove(InputAction.CallbackContext context)
    {
        playerData.moveVector = context.ReadValue<Vector2>();
        playerData.CurState = State.Move;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        playerData.moveVector = Vector2.zero;
        playerData.CurState = State.Idle;
    }
    #endregion

    private void OnJump(InputAction.CallbackContext context)
    {
        //float pressAnalog = 0f; //Ű�� ������� ������ �ִ���
        //if (context.phase == InputActionPhase.Performed)
        //{
        //    //����
        //    pressAnalog += Time.deltaTime;
        //}
        //else if (context.phase == InputActionPhase.Canceled)
        //{
        //    pressAnalog = 0f;
        //}
        //anim state -> jump
    }
}
