using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //컴포넌트
    public Rigidbody playerRgdby { get; private set; }  //플레이어 기본 물리
    public Animator animator { get; private set; }  //캐릭터 애니메이터
    public CapsuleCollider playerCollide { get; private set; }  //플레이어 충돌 (죽으면 충돌 무시)
    private PlayerInput canInput;   //입력 활성화
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
    private State playerState = State.Idle;
    public int CurrentId { get; set; }  //플레이어의 현재 아이디
    private bool imEnable = false;  //내가 사용자다!!!

    /// <summary>
    /// 컴포넌트 정의
    /// </summary>
    private void Awake()
    {
        //먼저 컴포넌트를 가져오고 아래 필요한 클래스 생성자로 전달
        playerRgdby = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        playerCollide = GetComponent<CapsuleCollider>();
        canInput = GetComponent<PlayerInput>();

        characterRotate = GetComponentInChildren<CharacterRotate>();
        health = GetComponentInChildren<PlayerHealth>();

        damage = gameObject.AddComponent<PlayerDamage>();

        //애니메이션 상태
        animState = new(this);

        //Rigidbody
        addCtrl = new (playerRgdby, playerSpeed);
        velCtrl = new (playerRgdby, slideFactor);

        playerRgdby.freezeRotation = true; // Rigidbody의 회전을 잠가 직접 회전 제어

        canInput.enabled = false;

        //gamePacket Payload (기본 초기화 후 서버에 전송)
        sendPlayerData.IcePlayerMoveRequest = new()
        {
            PlayerId = CurrentId,
            Rotation = characterRotate.transform.rotation.y,
            Vector = SocketManager.CreateVector(playerPos),
            Position = SocketManager.CreateVector(playerPos), //이거는 생성자 필요할 듯
            State = playerState
        };
    }

    private void Start()
    {

        //내 플레이어에 맞게
        //서버에서 아이디 정보를 받아서 해당 캐릭터 생성
        //CurrentId = gamePacket.IcePlayerSpawnNotification.PlayerId;

        //애니메이션 및 상태 초기
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
            characterRotate.SetInput(playerMoveInput);
        }

        //움직인걸 서버에 전송
        if (SocketManager.Instance != null)  //방어코드
        {
            if (imEnable)
                SendPosition();
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

    /// <summary>
    /// 내가 이 유저라면 입력 활성
    /// </summary>
    public void EnablePlayer()
    {
        this.enabled = true;
        canInput.enabled = true;
        imEnable = true;
        //내가 참가했다고 패킷에 전송

        sendPlayerData.IcePlayerMoveRequest.PlayerId = CurrentId;
        SendPosition();

    }

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

    /// <summary>
    /// 변경된 값을 서버에 전송
    /// </summary>
    public void SendPosition()
    {
        GamePacket packet = new();

        packet.IcePlayerMoveRequest = new()
        {
            //PlayerId = IceBoardPlayerManager.instan
            Position = SocketManager.CreateVector(transform.position),
            Force = SocketManager.CreateVector(addCtrl.GetForce()),
            Rotation = characterRotate.transform.rotation.y,
            State = playerState
        };

        SocketManager.Instance.OnSend(packet);
    }

    /// <summary>
    /// 받은 패킷을 Transform에 적용
    /// </summary>
    /// <param name="dir"></param>
    public void ReceivePosition(GamePacket packet)
    {
        var response = packet.IcePlayerMoveNotification;

        Vector3 getPos = Vector3.zero;
        Vector3 getForce = Vector3.zero;
        float getRot = 0f;

        if (response == null)
            return;
        else
        {
            for (int i = 0; i < response.Players.Count; i++)
            {
                Debug.Log(response.Players);
                if (response.Players[i].PlayerId == CurrentId)
                {
                    getPos.x = response.Players[i].Position.X;
                    getPos.y = response.Players[i].Position.Y;
                    getPos.z = response.Players[i].Position.Z;

                    getRot = response.Players[i].Rotation;

                    getForce.x = response.Players[i].Vector.X;
                    getForce.y = response.Players[i].Vector.Y;
                    getForce.z = response.Players[i].Vector.Z;

                    transform.position = getPos;
                    characterRotate.SetRotationY(getRot);
                    //addCtrl.SetForce(getForce);
                    playerState = response.Players[i].State;
                }
            }
        }
    }
}
