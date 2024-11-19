using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //������Ʈ
    private Rigidbody playerRgdby;  //�÷��̾� �⺻ ����
    public Animator animator { get; private set; }  //ĳ���� �ִϸ�����
    private CharacterRotate characterRotate;

    //��� Ŭ����
    IController curCtrl;
    private AddForceController addCtrl;
    private VelocityController velCtrl;
    private ButtonController btnCtrl = new ();
    PlayerAnimState animState;  //AnimState�� ��� (��ӽ��Ѽ�)

    //�÷��̾� ����
    private Vector3 playerPos = Vector3.zero;   //���� �÷��̾��� ��ġ
    private Vector2 playerMoveInput;   //�÷��̾� Move �Է�

    [Header("�÷��̾� �Ӽ�")]
    [SerializeField] private float playerSpeed = 10f; //�̵� �ӵ�
    [SerializeField] private float slideFactor = 1f; //�̲������� ���� ����

    /// <summary>
    /// ������Ʈ ����
    /// </summary>
    private void Awake()
    {
        //���� ������Ʈ�� �������� �Ʒ� �ʿ��� Ŭ���� �����ڷ� ����
        playerRgdby = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        characterRotate = GetComponentInChildren<CharacterRotate>();

        //�ִϸ��̼� ����
        animState = new(this);

        //Rigidbody
        addCtrl = new (playerRgdby, playerSpeed);
        velCtrl = new (playerRgdby, slideFactor);
    }

    private void Start()
    {
        playerRgdby.freezeRotation = true; // Rigidbody�� ȸ���� �ᰡ ���� ȸ�� ����

        animState.ChangeAnimation(animState.IdleAnim);  //�ִϸ��̼� �ʱ�ȭ
    }

    private void FixedUpdate()
    {
        BasicMove(playerMoveInput);
        characterRotate.SetInput(playerMoveInput);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Ư�� ��ġ �ȿ� ������ ������ ����
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            //IsDamage = true;    //������ �߻�
            if (collision.gameObject.name.Equals("20001"))   //�̰Ŵ� ���߿� ID�� ���� ������
            {
                //ƨ�ܳ�����
                BounceOut(collision);

                //0.2�ʰ� ����
                //_damage.GetStun(2f);
            }

            //������ ó��
            //StartCoroutine(_damage.DamageDelay(1));
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            //IsDamage = false;
            //StopCoroutine(_damage.DamageDelay(1));  //�ڷ�ƾ ������
        }
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
            playerMoveInput = context.ReadValue<Vector2>();
        }
        else if(context.phase.Equals(InputActionPhase.Canceled))
        {
            playerMoveInput = Vector2.zero;
        }

        ApplyAnimation();
    }
    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        float pressAnalog = 0f; //Ű�� ������� ������ �ִ���

        if(context.phase == InputActionPhase.Performed)
        {
            //����
            pressAnalog += Time.deltaTime;
            addCtrl.Jump();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            pressAnalog = 0f;
        }
    }
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    //�÷��̾� ���� �Ӽ�
    /// <summary>
    /// ���� �Է��� ��ġ�� ����
    /// </summary>
    /// <param name="dir">WSAD �Է�</param>
    private void BasicMove(Vector2 dir)
    {
        // WASD�� �Է¹޾� 3D�� ����Ʈ
        playerPos = transform.forward * dir.y + transform.right * dir.x;

        
        // IceSliding ���ַ� �۾������� �̰� �´��� �𸣰ڴ�....

        addCtrl.Move(playerPos);    //�̲����� ȿ��
        velCtrl.Move(playerRgdby.velocity); // ���� ȿ��

    }

    //�ִϸ��̼� ����
    /// <summary>
    /// �Է¿� ���� �ִϸ��̼� ����
    /// </summary>
    private void ApplyAnimation()
    {
        //�ִϸ��̼�
        if (playerMoveInput != Vector2.zero)
            animState.ChangeAnimation(animState.RunAnim);
        else
            animState.ChangeAnimation(animState.IdleAnim);
    }

    //�̰͵� ���� ���ߵ�
    /// <summary>
    /// ƨ�ܳ����� ȿ��
    /// </summary>
    /// <param name="collision">OnCollision����</param>
    private void BounceOut(Collision collision)
    {
        float bounceForce = 5f; // ƨ�ܳ����� ���� ����  (60�� ������ ������ ��������)
                                // �浹 ������ ���� ���� �������� (ù ��° ���� ������ ����)
        Vector3 collisionNormal = collision.GetContact(0).normal;

        // �����¿� ���� �� ���� ����� ������ bounceDirection���� ����
        Vector3 bounceDirection = new Vector3(
            Mathf.Round(collisionNormal.x),
            Mathf.Round(collisionNormal.y),
            Mathf.Round(collisionNormal.z)
        );

        // ������ �����Ǿ����� ƨ�ܳ����� �� ����
        if (bounceDirection != Vector3.zero)
        {
            playerRgdby.velocity = Vector3.zero; // ���� �ӵ� �ʱ�ȭ
            playerRgdby.AddForce(bounceDirection * bounceForce, ForceMode.VelocityChange);
        }
    }
}