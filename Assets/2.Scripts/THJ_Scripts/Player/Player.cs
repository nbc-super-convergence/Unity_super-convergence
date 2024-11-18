using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    //������Ʈ
    private Rigidbody playerRgdby;

    //��� Ŭ����
    IController curCtrl;
    private AddForceController addCtrl;
    private VelocityController velCtrl;
    private ButtonController btnCtrl = new ();
    //AnimState

    //�÷��̾� ����
    private Vector3 playerPos = Vector3.zero;   //���� �÷��̾��� ��ġ
    private Vector2 playerMov;   //�÷��̾� Move �Է�

    [Header("�÷��̾� �Ӽ�")]
    [SerializeField] private float playerSpeed = 10f; //�̵� �ӵ�
    [SerializeField] private float slideFactor = 1f; //�̲������� ���� ����

    /// <summary>
    /// ������Ʈ ����
    /// </summary>
    private void Awake()
    {
        //���� ������Ʈ�� �������� �Ʒ� Ŭ���� �����ڷ� ����
        playerRgdby = GetComponent<Rigidbody>();

        addCtrl = new (playerRgdby, playerSpeed);
        velCtrl = new (playerRgdby, slideFactor);
    }

    private void Start()
    {
        playerRgdby.freezeRotation = true; // Rigidbody�� ȸ���� �ᰡ ���� ȸ�� ����
    }

    private void FixedUpdate()
    {
        BasicMove(playerMov);
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="newCtrl"></param>
    public void ChangeState(IController newCtrl)
    {
        curCtrl = newCtrl;
    }

    //��Ʈ�ѷ� �Ӽ� (�̰� Ŭ���� ���� ���� �ǳ�?)
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
            //����

            addCtrl.Jump();
        }
    }

    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        bool isPress;
    }

    //�÷��̾� ���� �Ӽ�
    private void BasicMove(Vector2 dir)
    {
        // WASD�� �Է¹޾� 3D�� ����Ʈ
        playerPos = transform.forward * dir.y + transform.right * dir.x;

        
        // IceSliding ���ַ� �۾������� �̰� �´��� �𸣰ڴ�....

        addCtrl.Move(playerPos);    //�̲����� ȿ��

        velCtrl.Move(playerRgdby.velocity); // ���� ȿ��
    }
}
