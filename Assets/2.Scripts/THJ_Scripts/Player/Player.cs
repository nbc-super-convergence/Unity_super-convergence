using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    //컴포넌트
    private Rigidbody playerRgdby;

    //사용 클래스
    IController curCtrl;
    private AddForceController addCtrl;
    private VelocityController velCtrl;
    private ButtonController btnCtrl = new ();
    //AnimState

    //플레이어 벡터
    private Vector3 playerPos = Vector3.zero;   //현재 플레이어의 위치
    private Vector2 playerMov;   //플레이어 Move 입력

    [Header("플레이어 속성")]
    [SerializeField] private float playerSpeed = 10f; //이동 속도
    [SerializeField] private float slideFactor = 1f; //미끄러짐의 감속 비율

    /// <summary>
    /// 컴포넌트 정의
    /// </summary>
    private void Awake()
    {
        //먼저 컴포넌트를 가져오고 아래 클래스 생성자로 전달
        playerRgdby = GetComponent<Rigidbody>();

        addCtrl = new (playerRgdby, playerSpeed);
        velCtrl = new (playerRgdby, slideFactor);
    }

    private void Start()
    {
        playerRgdby.freezeRotation = true; // Rigidbody의 회전을 잠가 직접 회전 제어
    }

    private void FixedUpdate()
    {
        BasicMove(playerMov);
    }

    /// <summary>
    /// 상태 변경
    /// </summary>
    /// <param name="newCtrl"></param>
    public void ChangeState(IController newCtrl)
    {
        curCtrl = newCtrl;
    }

    //컨트롤러 속성 (이거 클래스 따로 빼야 되나?)
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            playerMov = context.ReadValue<Vector2>();
        }
        else if(context.phase.Equals(InputActionPhase.Canceled))
        {
            playerMov = Vector2.zero;
        }
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            //점프

            addCtrl.Jump();
        }
    }

    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        bool isPress;
    }

    //플레이어 동작 속성
    private void BasicMove(Vector2 dir)
    {
        // WASD로 입력받아 3D로 컨버트
        playerPos = transform.forward * dir.y + transform.right * dir.x;

        
        // IceSliding 위주로 작업했지만 이게 맞는지 모르겠다....

        addCtrl.Move(playerPos);    //미끄러짐 효과

        velCtrl.Move(playerRgdby.velocity); // 감속 효과
    }
}
