using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniPlayer : MonoBehaviour
{
    /*Controllers*/
    //IController curCtrl;

    public Vector3 nextPos;

    [Header("Components")]
    public Rigidbody rb;
    public CapsuleCollider _collider;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MiniPlayerRotate miniRotate;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    public State curState;

    [Header("Player Properties")]
    [SerializeField] private int MiniPlayerId;
    [SerializeField] private float forceMultiplier = 10f; //플레이어 속도
    [SerializeField] private float stopThreshold = 0.01f; //속도 기반 멈춤 판별
    private Vector2 moveInput;
    public Vector3 curForce;

    private bool IsClient => MiniPlayerId == GameManager.Instance.PlayerId;

    #region Unity Messages
    private void Awake()
    {
        playerInput.enabled = false;
        curState = State.Idle;
    }

    private void Start()
    {
        //StartCoroutine(SendMessage());
    }

    private void FixedUpdate()
    {
        if (IsClient)
        {
            MoveByInput(moveInput);
            miniRotate.InputRotation(moveInput);
        }   
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, 30 * Time.fixedDeltaTime * Vector3.Distance(transform.position, nextPos));
        }
    }       
    #endregion

    #region Server
    /// <summary>
    /// IcePlayerSpawnNotification Receive받기.
    /// </summary>
    public void ReceivePlayerSpawn(Vector3 position, float rotation)
    {
        playerInput.enabled = true; //Input 활성화
        transform.position = position; //position 초기화
        miniRotate.RotByReceive(rotation); //rotation 초기화
    }

    /// <summary>
    /// IcePlayerMoveRequest Send하기.
    /// </summary>
    private IEnumerator SendClientMove()
    {
        while (true)
        {
            Debug.LogError($"SendClientMove : {MiniPlayerId}");
            GamePacket packet = new()
            {
                IcePlayerMoveRequest = new()
                {
                    PlayerId = MiniPlayerId,
                    Position = SocketManager.ConvertVector(transform.position),
                    Force = SocketManager.ConvertVector(curForce),
                    Rotation = miniRotate.transform.rotation.y,
                    State = curState
                }
            };
            SocketManager.Instance.OnSend(packet);
            yield return new WaitForSeconds(0.1f);
        }   
    }

    /// <summary>
    /// IcePlayerMoveNotification Receive받기.
    /// </summary>
    public void ReceiveOtherMove(Vector3 pos, Vector3 force, float rotY, State state)
    {
        //transform.position = pos; //위치 동기화?
        MoveByReceive(pos, force);
        miniRotate.RotByReceive(rotY);
        curState = state;
    }
    #endregion

    public Coroutine c;

    #region Input System Events
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            moveInput = context.ReadValue<Vector2>();
            animator.SetBool("Move", true);
            curState = State.Move;
            c ??= StartCoroutine(SendClientMove());
        }
        else if(context.phase.Equals(InputActionPhase.Canceled))
        {
            moveInput = Vector2.zero;
            animator.SetBool("Move", false);
            curState = State.Idle;
        }
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        float pressAnalog = 0f; //키를 어느정도 누르고 있는지

        if(context.phase == InputActionPhase.Performed)
        {
            //점프
            pressAnalog += Time.deltaTime;
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
    #endregion

    #region Move
    /// <summary>
    /// Input에 따른 플레이어 움직임
    /// </summary>
    private void MoveByInput(Vector2 moveInput)
    {
        // WASD로 입력받아 3D로 컨버트
        Vector3 force = new(moveInput.x, 0, moveInput.y);
        rb.AddForce(force * forceMultiplier, ForceMode.Force);
        curForce = force;
    }
    /// <summary>
    /// Receive에 따른 플레이어 움직임
    /// </summary>
    private void MoveByReceive(Vector3 pos, Vector3 force)
    {
        if (!IsClient)
        {
            nextPos = pos;
        }
    }
    #endregion
}
