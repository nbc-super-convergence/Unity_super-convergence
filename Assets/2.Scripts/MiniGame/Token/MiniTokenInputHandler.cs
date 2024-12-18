using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniTokenInputHandler : IDisposable
{
    private PlayerInput playerInput;
    private MiniTokenData miniData;

    private List<InputActionMap> prevEnableMap = new List<InputActionMap>();
    public bool isEnable = true;
    public bool isDisposed = false;

    public MiniTokenInputHandler(MiniTokenData data)
    {
        playerInput = new PlayerInput();
        miniData = data;
    }

    public void Dispose()
    {
        if (isDisposed) return;

        if (playerInput != null)
        {
            playerInput.Dispose();
            playerInput = null;
        }

        miniData = null;

        isDisposed = true;
    }

    public void EnablePlayerInput()
    {
        playerInput.Enable();
        playerInput.SimpleInput.Disable();

        playerInput.MiniPlayerToken.Move.performed += OnMove;
        playerInput.MiniPlayerToken.Move.canceled += OnMoveCanceled;
    }

    public void DisablePlayerInput()
    {
        playerInput.Disable();

        playerInput.MiniPlayerToken.Move.performed -= OnMove;
        playerInput.MiniPlayerToken.Move.canceled -= OnMoveCanceled;
    }

    public void EnableSimpleInput()
    {
        playerInput.Enable();
        playerInput.MiniPlayerToken.Disable();

        playerInput.SimpleInput.Up.started += OnUp;
        playerInput.SimpleInput.Left.started += OnLeft;
        playerInput.SimpleInput.Down.started += OnDown;
        playerInput.SimpleInput.Right.started += OnRight;
    }

    public void DisableSimpleInput()
    {
        playerInput.Disable();
        playerInput.MiniPlayerToken.Disable();

        playerInput.SimpleInput.Up.started -= OnUp;
        playerInput.SimpleInput.Left.started -= OnLeft;
        playerInput.SimpleInput.Down.started -= OnDown;
        playerInput.SimpleInput.Right.started -= OnRight;
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

    #region SimpleInput   
    private void OnUp(InputAction.CallbackContext context)
    {
        if (isEnable)
        {
            miniData.arrowInput = 0;
            if (UIManager.IsOpened<UICourtshipDance>())
            {
                UIManager.Get<UICourtshipDance>().myBoard.OnActionInput(miniData.arrowInput);
            }

            Debug.Log($"ActionMap MiniToken : {playerInput.MiniPlayerToken.enabled}");
            Debug.Log($"ActionMap SimpleInput : {playerInput.SimpleInput.enabled}");
        }
    }
    private void OnLeft(InputAction.CallbackContext context)
    {
        if (isEnable)
        {
            miniData.arrowInput = 90;
            if (UIManager.IsOpened<UICourtshipDance>())
            {
                UIManager.Get<UICourtshipDance>().myBoard.OnActionInput(miniData.arrowInput);
            }
        }
    }
    private void OnDown(InputAction.CallbackContext context)
    {
        if (isEnable)
        {
            miniData.arrowInput = 180;
            if (UIManager.IsOpened<UICourtshipDance>())
            {
                UIManager.Get<UICourtshipDance>().myBoard.OnActionInput(miniData.arrowInput);
            }
        }
    }
    private void OnRight(InputAction.CallbackContext context)
    {
        if (isEnable)
        {
            miniData.arrowInput = 270;
            if (UIManager.IsOpened<UICourtshipDance>())
            {
                UIManager.Get<UICourtshipDance>().myBoard.OnActionInput(miniData.arrowInput);
            }
        }
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
