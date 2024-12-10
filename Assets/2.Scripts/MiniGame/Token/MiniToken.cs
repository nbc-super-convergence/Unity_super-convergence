using Sirenix.OdinInspector.Editor;
using System.Collections;
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

    Coroutine PauseInput = null;

    #region Unity Messages
    private void Awake()
    {//BoardScene 진입 시 일어나는 초기화.
        MiniData = new(animator, MyColor);
        InputHandler = new(MiniData);
        Controller = new MiniTokenController(MiniData, transform, rb);
        IsClient = MiniData.tokenColor == GameManager.Instance.SessionDic[MinigameManager.Instance.mySessonId].Color;
        InputHandler.ChangeActionMap("MiniPlayerToken");
    }

    private void Update()
    {
        if (!IsClient && isEnabled)
        {
            switch (MinigameManager.gameType)
            {
                case eGameType.GameIceSlider:
                    Controller.MoveToken(eMoveType.Server);
                    break;
                case eGameType.GameBombDelivery:
                    Controller.MoveToken(eMoveType.Server);
                    break;
                case eGameType.GameCourtshipDance:
                    // 서버에서 토큰 무브에 관련된 정보를 받을 필요는 없음.   // TODO::나중에 봐서 이부분 지워도되면 지우기.
                    break;
                case eGameType.GameDropper:
                    Controller.MoveToken(eMoveType.Dropper);
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
                        Controller.MoveToken(eMoveType.AddForce);
                        break;
                    case eGameType.GameBombDelivery:
                        Controller.MoveToken(eMoveType.Velocity);
                        break;
                    case eGameType.GameDropper:
                        Controller.MoveToken(eMoveType.Dropper);
                        break;
                }
            }

            Controller.RotateToken(MiniData.rotY);
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
                            Debug.Log("GameBombDelivery");
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
    public void ReceiveOtherMove(Vector3 pos, Vector3 force, float rotY, State state)
    {
        MiniData.nextPos = pos;
        MiniData.rotY = rotY;
        MiniData.CurState = state;
    }
    #endregion

    #region CourtshipDance
    public void EnableInputSystem(eGameType eGameType)
    {
        if (IsClient)
        {//방어코드
            InputHandler.EnablePlayerInput();
            //StartCoroutine(SendClientMove());
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
        InputHandler.DisablePlayerInput();
    }

    public void DisableMiniToken()
    {
        isEnabled = false;
        rb.velocity = Vector3.zero; //움직임 멈춤
        MiniData.CurState = State.Die; //사망 애니메이션 재생
        StartCoroutine(DisableDelay());
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
        InputHandler.DisablePlayerInput();

        yield return new WaitForSeconds(1.5f);

        InputHandler.EnablePlayerInput();
    }
}