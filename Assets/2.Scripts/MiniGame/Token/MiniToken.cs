using System.Collections;
using UnityEngine;

public class MiniToken : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    /*Data*/
    public MiniTokenData MiniData;

    /*Input & Control*/
    public MiniTokenInputHandler InputHandler { get; private set; }
    public MiniTokenController Controller { get; private set; }

    /*Server*/
    public bool IsClient { get; private set; }
    private bool isEnabled = false;

    Coroutine PauseInput = null;

    #region Unity Messages
    private void Awake()
    {//BoardScene 진입 시 일어나는 초기화.
        MiniData = new(animator);
        InputHandler = new(MiniData);
        Controller = new MiniTokenController(MiniData, transform, rb);
        IsClient = MiniData.miniTokenId == GameManager.Instance.SessionDic[MinigameManager.Instance.MySessonId].Color;
    }

    private void Update()
    {
        if (!IsClient && isEnabled)
        {
            switch (MinigameManager.GameType)
            {
                case eGameType.GameIceSlider:
                    Controller.MoveToken(eMoveType.Server);
                    Controller.SetRotY(MiniData.rotY);
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsClient && isEnabled)
        {
            switch (MinigameManager.GameType)
            {
                case eGameType.GameIceSlider:
                    Controller.MoveToken(eMoveType.AddForce);
                    Controller.SetRotY(MiniData.rotY);
                    break;
            }
        }
    }
    #endregion

    public void PausePlayerInput(float pauseTime)
    {
        if (PauseInput != null)
        {
            StopCoroutine(PauseInput);
        }
        PauseInput = StartCoroutine(InputHandler.PauseCotoutine(pauseTime));
    }

    #region IceBoard
    public void EnableInputSystem()
    {
        if (IsClient)
        {//방어코드
            InputHandler.EnablePlayerInput();
            StartCoroutine(SendClientMove());
        }
    }

    private IEnumerator SendClientMove()
    {
        Vector3 curPos = transform.localPosition, lastPos = transform.localPosition;
        while (true)
        {
            curPos = transform.localPosition;
            if (curPos != lastPos)
            {
                GamePacket packet = new();
                {
                    packet.IcePlayerSyncRequest = new()
                    {
                        SessionId = MinigameManager.Instance.MySessonId,
                        Position = SocketManager.ToVector(transform.localPosition),
                        Rotation = transform.rotation.y,
                        State = MiniData.CurState
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
    #endregion

    public void EnableMiniToken()
    {
        gameObject.SetActive(true);
        isEnabled = true;
    }

    public void DisableMyToken()
    {
        InputHandler.DisablePlayerInput();
    }

    public void DisableMiniToken()
    {
        isEnabled = false;
        rb.velocity = Vector3.zero; //움직임 멈춤
        MiniData.CurState = State.Die; //사망 애니메이션 재생
        StartCoroutine(disableDelay());
    }

    private IEnumerator disableDelay()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}