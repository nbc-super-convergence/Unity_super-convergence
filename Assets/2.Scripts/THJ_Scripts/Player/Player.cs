using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //컴포넌트
    private Rigidbody playerRgdby;  //플레이어 기본 물리
    public Animator animator { get; private set; }  //캐릭터 애니메이터
    private CharacterRotate characterRotate;

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
        characterRotate = GetComponentInChildren<CharacterRotate>();

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

    private void FixedUpdate()
    {
        BasicMove(playerMoveInput);
        characterRotate.SetInput(playerMoveInput);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //특정 위치 안에 있으면 데미지 감소
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            //IsDamage = true;    //데미지 발생
            if (collision.gameObject.name.Equals("20001"))   //이거는 나중에 ID로 받을 예정임
            {
                //튕겨나가기
                BounceOut(collision);

                //0.2초간 스턴
                //_damage.GetStun(2f);
            }

            //데미지 처리
            //StartCoroutine(_damage.DamageDelay(1));
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            //IsDamage = false;
            //StopCoroutine(_damage.DamageDelay(1));  //코루틴 끝내기
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

    //이것도 따로 빼야됨
    /// <summary>
    /// 튕겨나가는 효과
    /// </summary>
    /// <param name="collision">OnCollision에서</param>
    private void BounceOut(Collision collision)
    {
        float bounceForce = 5f; // 튕겨나가는 힘의 세기  (60초 지나면 강도가 강해지게)
                                // 충돌 지점의 법선 벡터 가져오기 (첫 번째 접촉 지점의 법선)
        Vector3 collisionNormal = collision.GetContact(0).normal;

        // 상하좌우 방향 중 가장 가까운 방향을 bounceDirection으로 설정
        Vector3 bounceDirection = new Vector3(
            Mathf.Round(collisionNormal.x),
            Mathf.Round(collisionNormal.y),
            Mathf.Round(collisionNormal.z)
        );

        // 방향이 설정되었으면 튕겨나가는 힘 적용
        if (bounceDirection != Vector3.zero)
        {
            playerRgdby.velocity = Vector3.zero; // 기존 속도 초기화
            playerRgdby.AddForce(bounceDirection * bounceForce, ForceMode.VelocityChange);
        }
    }
}