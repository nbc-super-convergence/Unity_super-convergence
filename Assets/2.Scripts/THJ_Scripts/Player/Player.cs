using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //컴포넌트
    public Rigidbody playerRgdby { get; private set; }  //플레이어 기본 물리
    public Animator animator { get; private set; }  //캐릭터 애니메이터
    public CapsuleCollider playerCollide { get; private set; }  //플레이어 충돌 (죽으면 충돌 무시)
    private CharacterRotate characterRotate;
    private PlayerHealth health;
    private PlayerDamage damage;

    //사용 클래스
    IController curCtrl;
    private AddForceController addCtrl;
    private VelocityController velCtrl;
    private ButtonController btnCtrl = new ();
    PlayerAnimState animState;  //AnimState를 대신 (상속시켜서)

    //플레이어 벡터
    private Vector3 playerPos = Vector3.zero;   //현재 플레이어의 위치
    private Vector2 playerMoveInput;   //플레이어 Move 입력

    [Header("플레이어 속성")]
    [SerializeField] private float playerSpeed = 10f; //이동 속도
    [SerializeField] private float slideFactor = 1f; //미끄러짐의 감속 비율

    /// <summary>
    /// 컴포넌트 정의
    /// </summary>
    private void Awake()
    {
        //먼저 컴포넌트를 가져오고 아래 필요한 클래스 생성자로 전달
        playerRgdby = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        playerCollide = GetComponent<CapsuleCollider>();

        characterRotate = GetComponentInChildren<CharacterRotate>();
        health = GetComponentInChildren<PlayerHealth>();

        damage = gameObject.AddComponent<PlayerDamage>();

        //애니메이션 상태
        animState = new(this);

        //Rigidbody
        addCtrl = new (playerRgdby, playerSpeed);
        velCtrl = new (playerRgdby, slideFactor);
    }

    private void Start()
    {
        playerRgdby.freezeRotation = true; // Rigidbody의 회전을 잠가 직접 회전 제어

        animState.ChangeAnimation(animState.IdleAnim);  //애니메이션 초기화
    }

    private void Update()
    {
        if(!damage.IsAlive)
        {
            animState.Update();
            animState.ChangeAnimation(animState.DeathAnim);
        }
    }

    private void FixedUpdate()
    {
        //상태 이상이 없으면
        if (damage.IsAlive && !damage.IsStun)
        {
            BasicMove(playerMoveInput);
            characterRotate.SetInput(playerMoveInput);
        }
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
        float pressAnalog = 0f; //키를 어느정도 누르고 있는지

        if(context.phase == InputActionPhase.Performed)
        {
            //점프
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

    //플레이어 동작 속성
    /// <summary>
    /// 받은 입력을 위치에 적용
    /// </summary>
    /// <param name="dir">WSAD 입력</param>
    private void BasicMove(Vector2 dir)
    {
        // WASD로 입력받아 3D로 컨버트
        playerPos = transform.forward * dir.y + transform.right * dir.x;

        
        // IceSliding 위주로 작업했지만 이게 맞는지 모르겠다....

        addCtrl.Move(playerPos);    //미끄러짐 효과
        velCtrl.Move(playerRgdby.velocity); // 감속 효과

    }

    //애니메이션 적용
    /// <summary>
    /// 입력에 따른 애니메이션 적용
    /// </summary>
    private void ApplyAnimation()
    {
        //애니메이션
        if (playerMoveInput != Vector2.zero)
            animState.ChangeAnimation(animState.RunAnim);
        else
            animState.ChangeAnimation(animState.IdleAnim);
    }
}
