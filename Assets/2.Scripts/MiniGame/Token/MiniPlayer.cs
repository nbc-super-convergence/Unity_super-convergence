using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MiniPlayer : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private MiniPlayerTokenData playerData;
    
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private Animator animator;
    [SerializeField] private MiniPlayerRotate miniRotate;

    [Header("Input & Control")]
    [SerializeField] private MiniPlayerInputHandler inputHandler;
    private MiniPlayerController curCtrl;
    
    /*Server*/
    private bool IsClient => playerData.miniPlayerId == GameManager.Instance.PlayerId;
    private Coroutine SendMoveCoroutine;
    private Vector3 nextPos; //수동 움직임.

    #region Unity Messages
    private void Awake()
    {
        inputHandler.Init(playerData);
    }

    private void Update()
    {
        if (!IsClient)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, 30 * Time.deltaTime * Vector3.Distance(transform.position, nextPos));
        }
    }

    private void FixedUpdate()
    {
        if (IsClient)
        {
            if (playerData.WASDInput)
            {
                curCtrl.MoveVector2();
                miniRotate.InputRotation(playerData.moveVector);
            }
        }
    }       
    #endregion

    #region Server
    /// <summary>
    /// IcePlayerSpawnNotification Receive받기.
    /// </summary>
    public void ReceivePlayerSpawn(Vector3 position, float rotation)
    {
        inputHandler.EnablePlayerInput();
        //TODO: Input Map 바꾸기...?
        curCtrl = new AddForceController(playerData, rb); //컨트롤러 고르기.
        transform.position = position; //position 초기화
        miniRotate.RotByReceive(rotation); //rotation 초기화
        SendMoveCoroutine ??= StartCoroutine(SendClientMove());
    }

    /// <summary>
    /// IcePlayerMoveRequest Send하기.
    /// </summary>
    private IEnumerator SendClientMove()
    {
        Vector3 curPos = transform.position, lastPos = transform.position;
        while (true)
        {
            curPos = transform.position;
            if (curPos != lastPos)
            {
                GamePacket packet = new()
                {
                    IcePlayerMoveRequest = new()
                    {
                        PlayerId = playerData.miniPlayerId,
                        Position = SocketManager.ConvertVector(transform.position),
                        Rotation = miniRotate.transform.rotation.y,
                        //State = playerData.CurState
                    }
                };
                SocketManager.Instance.OnSend(packet);
                lastPos = curPos;
            }
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
        playerData.CurState = state;
    }
    
    public void ReceivePlayerDespawn()
    {
        inputHandler.DisablePlayerInput();
        curCtrl = null;
    }
    #endregion

    #region Move
    /// <summary>
    /// Input에 따른 플레이어 움직임
    /// </summary>
    private void MoveByInput(Vector2 moveInput)
    {
        
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
