using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChmChmOwner : ChmChmBase
{

    private void Start()
    {
        LookDirection = Vector3.zero;
    }

    /// <summary>
    /// 방향 입력
    /// </summary>
    /// <param name="context"></param>
    public void OnLook(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            LookDirection = context.ReadValue<Vector3>();

            if(LookDirection.x == 0)
                LookDirection = Vector3.zero;
        }
    }
}
