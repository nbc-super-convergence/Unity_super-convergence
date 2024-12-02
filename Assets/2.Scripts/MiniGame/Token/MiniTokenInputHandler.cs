using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniTokenInputHandler 
{
    private PlayerInput playerInput;
    private MiniTokenData miniData;

    public MiniTokenInputHandler(MiniTokenData data)
    {
        playerInput = new PlayerInput();
        miniData = data;
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

    

    public IEnumerator PauseCotoutine(float time)
    {
        playerInput.Disable();

        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerInput.Enable();
    }

    #region WASD 이동
    private void OnMove(InputAction.CallbackContext context)
    {
        miniData.wasdVector = context.ReadValue<Vector2>();
        miniData.rotY = Mathf.Atan2(miniData.wasdVector.x, miniData.wasdVector.y) * Mathf.Rad2Deg;
        miniData.CurState = State.Move;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        miniData.wasdVector = Vector2.zero;
        miniData.CurState = State.Idle;
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
