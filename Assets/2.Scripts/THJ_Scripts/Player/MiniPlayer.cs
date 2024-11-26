using UnityEngine;
using UnityEngine.InputSystem;

public class MiniPlayer : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public CapsuleCollider _collider;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MiniPlayerRotate miniRotate;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerDamage damage;

    /*Controllers*/
    IController curCtrl;
    private AddForceController addCtrl;
    private VelocityController velCtrl;
    private ButtonController btnCtrl = new();
    
    /*Animation State*/
    PlayerAnimState animState;  //AnimState를 대신 (상속시켜서)
    private State playerState = State.Idle;

    //플레이어 벡터
    private Vector3 playerPos = Vector3.zero;   //현재 플레이어의 위치
    private Vector2 playerMoveInput;   //플레이어 Move 입력

    [Header("플레이어 속성")]
    public int MiniPlayerId; //오브젝트 ID.
    [SerializeField] private float playerSpeed = 10f; //이동 속도
    [SerializeField] private float slideFactor = 1f; //감속 비율
    private bool IsClient => MiniPlayerId == GameManager.Instance.PlayerId;

    #region Unity Messages
    /// <summary>
    /// 컴포넌트 정의
    /// </summary>
    private void Awake()
    {
        //Rigidbody
        addCtrl = new(rb, playerSpeed);
        velCtrl = new(rb, slideFactor);

        //Animation State
        animState = new(this);

        //Disable Player Input
        playerInput.enabled = false;
    }

    private void Start()
    {
        animState.ChangeAnimation(animState.IdleAnim);
        playerState = State.Idle;
    }

    private void Update()
    {
        if(damage.IsAlive)
        {
            playerState = State.Idle;
        }
        else
        {
            playerState = State.Die;
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
            miniRotate.InputRotation(playerMoveInput);
        }

        //움직인걸 서버에 전송
        if (SocketManager.Instance != null)
        {
            if (IsClient)
                SendClientMove();
        }
    }
    #endregion

    /// <summary>
    /// Controller 변경
    /// </summary>
    /// <param name="newCtrl"></param>
    public void SetController(IController newCtrl)
    {
        curCtrl = newCtrl;
    }

    #region Server
    /// <summary>
    /// IcePlayerSpawnNotification Receive받기.
    /// </summary>
    public void ReceivePlayerSpawn(Vector3 position, float rotation)
    {
        playerInput.enabled = true; //Input 활성화
        transform.position = position; //position 초기화
        miniRotate.ReceiveRotation(rotation); //rotation 초기화
    }

    /// <summary>
    /// IcePlayerMoveRequest Send하기.
    /// </summary>
    public void SendClientMove()
    {
        GamePacket packet = new();

        packet.IcePlayerMoveRequest = new()
        {
            //PlayerId = IceBoardPlayerManager.instan
            Position = SocketManager.ConvertVector(transform.position),
            Force = SocketManager.ConvertVector(addCtrl.GetForce()),
            Rotation = miniRotate.transform.rotation.y,
            State = playerState
        };

        SocketManager.Instance.OnSend(packet);
    }

    /// <summary>
    /// IcePlayerMoveNotification Receive받기.
    /// </summary>
    public void ReceiveOtherMove(Vector3 pos, Vector3 force, float rotY, State state)
    {
        
    }
    #endregion

    //컨트롤러 속성 (이거 클래스 따로 빼야 되나?)
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            playerMoveInput = context.ReadValue<Vector2>();
            playerState = State.Move;
        }
        else if(context.phase.Equals(InputActionPhase.Canceled))
        {
            playerMoveInput = Vector2.zero;
            playerState = State.Idle;
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
        velCtrl.Move(rb.velocity); // 감속 효과
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
