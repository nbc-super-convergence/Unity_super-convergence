using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    IController curCtrl;
    private AddForceController addCtrl = new ();
    private VelocityController velCtrl = new ();
    private ButtonController btnCtrl = new ();
    //AnimState

    private Vector3 playerPos = Vector3.zero;

    public void ChangeState(IController newCtrl)
    {
        curCtrl = newCtrl;
    }

    //컨트롤러 속성 (이거 클래스 따로 빼야 되나?)
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            playerPos = context.ReadValue<Vector2>();

            addCtrl.Move(playerPos);
        }
        else if(context.phase.Equals(InputActionPhase.Canceled))
        {
            playerPos = Vector2.zero;
        }
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            //점프
        }
    }

    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            //상호작용
            //입력 동작이 다양할 텐데 이걸 어떻게 처리?
        }
    }
}
