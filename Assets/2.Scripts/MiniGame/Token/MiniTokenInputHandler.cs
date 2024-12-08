using System.Collections;
using System.Collections.Generic;
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

        playerInput.SimpleInput.Up.started += OnUp;
        playerInput.SimpleInput.Left.started += OnLeft;
        playerInput.SimpleInput.Down.started += OnDown;
        playerInput.SimpleInput.Right.started += OnRight;
    }

    public void DisablePlayerInput()
    {
        playerInput.Disable();

        playerInput.MiniPlayerToken.Move.performed -= OnMove;
        playerInput.MiniPlayerToken.Move.canceled -= OnMoveCanceled;

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

    // 현재 활성화된 액션맵을 Disable하고 매개변수의 액션맵을 Enable하는 기능.
    public void ChangeActionMap(string newActionMapName)
    {
        // playerInput.SwitchCurrentActionMap("SimpleInput"); 는 Unity PlayerInput을 GetComponent해야 사용할 수있는 메서드......

        // 액션맵 교체 전 활성화 되어있던 액션맵을 기억해두기.
        List<InputActionMap> prevEnableMap = new List<InputActionMap>();
        if(playerInput.MiniPlayerToken.enabled)
        {
            prevEnableMap.Add(playerInput.MiniPlayerToken);
        }
        if (playerInput.SimpleInput.enabled)
        {
            prevEnableMap.Add(playerInput.SimpleInput);
        }

        // 모든 액션맵 비활성화
        foreach ( InputActionMap actionMap in prevEnableMap)
        {
            actionMap.Disable();
        }

        var newActionMap = playerInput.FindAction(newActionMapName, true);
        if (newActionMap != null)
        {
            newActionMap.Enable();
        }
        else
        {
            foreach ( InputActionMap actionMap in prevEnableMap)
            {
                actionMap.Enable();
            }
        }
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
        miniData.arrowInput = 0;
    }

    private void OnLeft(InputAction.CallbackContext context)
    {
        miniData.arrowInput = 90;
    }
    private void OnDown(InputAction.CallbackContext context)
    {
        miniData.arrowInput = 180;
    }
    private void OnRight(InputAction.CallbackContext context)
    {
        miniData.arrowInput = 270;
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
