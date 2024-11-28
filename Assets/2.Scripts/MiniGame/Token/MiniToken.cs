using System.Collections;
using UnityEngine;

public class MiniToken : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    [Header("Data")]
    private MiniTokenData miniData;
 
    [Header("Input & Control")]
    private MiniTokenInputHandler inputHandler;
    private MiniTokenController controller;

    /*Server*/
    private bool IsClient;
    private Coroutine SendMoveCoroutine;

    #region Unity Messages
    private void Awake()
    {//BoardScene 진입 시 일어나는 초기화.
        miniData = new(animator);
        inputHandler = new(miniData);
        controller = new MiniTokenController(miniData, transform, rb);
    }

    private void Update()
    {
        if (!IsClient)
        {
            switch (MiniGameManager.Instance.type)
            {
                case eGameType.GameIceSlider:
                    controller.MoveVector2(eMoveType.Server);
                    controller.RotateY();
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsClient)
        {
            switch (MiniGameManager.Instance.type)
            {
                case eGameType.GameIceSlider:
                    controller.MoveVector2(eMoveType.AddForce);
                    controller.RotateY();
                    break;
            }
        }
    }       
    #endregion

    #region Server
    /// <summary>
    /// IcePlayerSpawnNotification Receive받기.
    /// </summary>
    public void ReceivePlayerSpawn(Vector3 position, float rotY)
    {
        inputHandler.EnablePlayerInput();
        //TODO: Input Map 바꾸기...?
         //컨트롤러 고르기.
        transform.position = position;
        miniData.rotY = rotY;
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
                        PlayerId = miniData.miniTokenId,
                        Position = SocketManager.ConvertVector(transform.position),
                        Rotation = transform.rotation.y,
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
        miniData.nextPos = pos;
        miniData.rotY = rotY;
        miniData.CurState = state;
    }
    
    public void ReceivePlayerDespawn()
    {
        inputHandler.DisablePlayerInput();
        controller = null;
    }
    #endregion
}