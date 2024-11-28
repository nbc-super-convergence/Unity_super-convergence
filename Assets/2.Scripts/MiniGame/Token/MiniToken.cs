using System.Collections;
using UnityEngine;

public class MiniToken : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    /*Data*/
    public MiniTokenData MiniData { get; set; }

    /*Input & Control*/
    public MiniTokenInputHandler InputHandler { get; private set; }
    public MiniTokenController Controller { get; private set; }

    /*Server*/
    private bool IsClient;
    private Coroutine SendMoveCoroutine;

    #region Unity Messages
    private void Awake()
    {//BoardScene 진입 시 일어나는 초기화.
        MiniData = new(animator);
        InputHandler = new(MiniData);
        Controller = new MiniTokenController(MiniData, transform, rb);
    }

    private void Update()
    {
        if (!IsClient)
        {
            switch (MinigameManager.Instance.type)
            {
                case eGameType.GameIceSlider:
                    Controller.MoveToken(eMoveType.Server);
                    Controller.RotateY(MiniData.rotY);
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsClient)
        {
            switch (MinigameManager.Instance.type)
            {
                case eGameType.GameIceSlider:
                    Controller.MoveToken(eMoveType.AddForce);
                    Controller.RotateY(MiniData.rotY);
                    break;
            }
        }
    }
    #endregion

    #region IceBoard
    //IceMiniGameStartNotification : SocketManager
    //

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
                GamePacket packet = new();
                {
                    packet.IcePlayerSyncRequest = new()
                    {
                        //PlayerId = MiniData.miniTokenId,
                        Position = SocketManager.ToVector(transform.position),
                        Rotation = transform.rotation.y,
                        //State = playerData.CurState
                    };
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
        MiniData.nextPos = pos;
        MiniData.rotY = rotY;
        MiniData.CurState = state;
    }
    
    public void ReceivePlayerDespawn()
    {
        InputHandler.DisablePlayerInput();
        Controller = null;
    }
    #endregion
}