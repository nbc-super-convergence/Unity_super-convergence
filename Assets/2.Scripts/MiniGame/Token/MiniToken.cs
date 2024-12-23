using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MiniToken : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    /*Data*/
    public int MyColor;
    public MiniTokenData MiniData;

    /*Input & Control*/
    public MiniTokenInputHandler InputHandler { get; private set; }
    public MiniTokenController Controller { get; private set; }

    /*Server*/
    public bool IsClient { get; private set; }
    private bool isEnabled = false;
    public bool isStun { get; private set; }
    Coroutine PauseInput = null;
    Coroutine SendMoveCoroutine = null;

    #region Unity Messages
    private void Awake()
    {//BoardScene 진입 시 일어나는 초기화.
        MiniData = new(animator, MyColor);
        InputHandler = new(MiniData);
        Controller = new MiniTokenController(MiniData, transform, rb);
        IsClient = MiniData.tokenColor == GameManager.Instance.SessionDic[MinigameManager.Instance.mySessonId].Color;
        InputHandler.DisableSimpleInput();
    }

    private void Update()
    {
        if (!IsClient && isEnabled)
        {
            switch (MinigameManager.gameType)
            {
                case eGameType.GameDropper:
                    Controller.MoveToken(eMoveType.Dropper);
                    break;
                case eGameType.GameDart:
                    Controller.MoveToken(eMoveType.Server);
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isEnabled)
        {
            if (IsClient)
            {
                switch (MinigameManager.gameType)
                {
                    case eGameType.GameIceSlider:
                        Controller.MoveToken(eMoveType.Ice);
                        break;
                    case eGameType.GameBombDelivery:
                        Controller.MoveToken(eMoveType.Velocity);
                        break;
                    case eGameType.GameDropper:
                        Controller.MoveToken(eMoveType.Dropper);
                        break;
                    case eGameType.GameDart:
                        Controller.MoveToken(eMoveType.Ice);
                        break;
                }
            }

            Controller.RotateToken(MiniData.rotY);
        }
    }

    private void OnDestroy()
    {
        if (InputHandler != null)
        {
            InputHandler.Dispose();
            InputHandler = null; 
        }

        Controller = null;
        MiniData = null;
        
        if (PauseInput != null)
        {
            StopCoroutine(PauseInput);
            PauseInput = null;
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
            SendMoveCoroutine = StartCoroutine(SendClientMove());
        }
    }

    private IEnumerator SendClientMove()
    {
        Vector3 curPos = transform.localPosition, lastPos = transform.localPosition;

        while (true)
        {
            if (MiniData.CurState == State.Die) yield break;

            curPos = transform.localPosition;
            if (curPos != lastPos)
            {
                GamePacket packet = new();
                {
                    switch(MinigameManager.gameType)
                    {
                        case eGameType.GameIceSlider:
                            packet.IcePlayerSyncRequest = new()
                            {
                                SessionId = MinigameManager.Instance.mySessonId,
                                Position = SocketManager.ToVector(transform.localPosition),
                                Rotation = transform.eulerAngles.y,
                                State = MiniData.CurState
                            };
                            break;

                        case eGameType.GameBombDelivery:
                            packet.BombPlayerSyncRequest = new()
                            {
                                SessionId = MinigameManager.Instance.mySessonId,
                                Position = SocketManager.ToVector(transform.localPosition),
                                Rotation = transform.eulerAngles.y,
                                State = MiniData.CurState
                            };
                            break;
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
    //public void ReceiveOtherMove(Vector3 pos, Vector3 force, float rotY, State state)
    //{
    //    MiniData.nextPos = pos;
    //    MiniData.rotY = rotY;
    //    MiniData.CurState = state;
    //}
    #endregion

    #region CourtshipDance
    public void EnableInputSystem(eGameType eGameType)
    {
        if (eGameType== eGameType.GameCourtshipDance)
        {
            InputHandler.DisablePlayerInput();
            InputHandler.EnableSimpleInput();
        }
    }
    public Animator GetAnimator()
    {
        return animator;
    }
    #endregion

    public void EnableMiniToken()
    {
        gameObject.SetActive(true);
        isEnabled = true;
    }

    public void DisableMyToken()
    {
        if (SendMoveCoroutine != null)
        {
            StopCoroutine(SendMoveCoroutine);
            SendMoveCoroutine = null;
        }

        InputHandler.DisablePlayerInput();
    }

    public void DisableMiniToken()
    {
        isEnabled = false;
        rb.velocity = Vector3.zero; //움직임 멈춤
        MiniData.CurState = State.Die; //사망 애니메이션 재생
        if(gameObject.activeSelf) StartCoroutine(DisableDelay());
    }

    private IEnumerator DisableDelay()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    public void Stun()
    {
        StartCoroutine(StunDelay());
    }

    private IEnumerator StunDelay()
    {
        isStun = true;
        InputHandler.DisablePlayerInput();

        yield return new WaitForSeconds(1.5f);

        InputHandler.EnablePlayerInput();
        isStun = false;
    }

    private readonly float moveDuration = 0.1f;
    public Coroutine ServerMoveCoroutine;
    public IEnumerator ServerMove()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, MiniData.nextPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = MiniData.nextPos;
        ServerMoveCoroutine = null;
    }
}