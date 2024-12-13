using UnityEngine;

public class DartPlayer : MonoBehaviour
{
    private GameDartEvent orderEvent;
    private Rigidbody rgdby;

    //서버 전송 데이터
    private DiceGameData diceData = new();
    public DiceGameData DiceGameData
    {
        get => diceData;
        private set
        {
            diceData = value;
        }
    }

    private bool isIncrease = true; //증감 여부
    private int actionPhase = 0;

    private float minAim, maxAim;
    private Vector3 aimVector;
    public Vector3 CurAim
    {
        get => aimVector;
        set
        {
            aimVector.x = Mathf.Clamp(value.x, minAim, maxAim);
            aimVector.y = Mathf.Clamp(value.y, minAim, maxAim);

            DiceGameData.Angle = SocketManager.ToVector(CurAim);
        }
    }
    public Vector2 GetAim = Vector2.zero;   //입력 Aim

    private float curForce = 2f;
    private float minForce, maxForce;
    public float CurForce
    {
        get => curForce;
        set
        {
            curForce = Mathf.Clamp(value, minForce, maxForce);
            DiceGameData.Power = curForce;

            if(curForce <= minForce)
                isIncrease = true;
            if (curForce >= maxForce)
                isIncrease = false;
        }
    }

    private float myDistance = 0f;
    public float MyDistance
    {
        get => myDistance;
        set
        {
            myDistance = value;
            DiceGameData.Distance = myDistance;
        }
    }

    private int myRank = 0;
    public int MyRank 
    {
        get => myRank;
        set
        {
            myRank = value;
            DiceGameData.Rank = myRank;
        }
    }

    public int MyColor;  //내 플레이어 인덱스

    //Server
    public bool IsClient { get; private set; }

    //나갈 각도
    private Vector3 dartRot = Vector3.back;

    private void Awake()
    {
        rgdby = GetComponent<Rigidbody>();
        orderEvent = GetComponent<GameDartEvent>();

        //이걸 Data클래스에서 받지 말고 그냥 여기서 설정하도록 할까?
        //UI만 보내는거 말고 없는것 같다......
        SetAimRange(-20f, 20f);
        SetForceRange(1.5f, 3f);

        IsClient = GameManager.Instance.SessionDic[MinigameManager.Instance.mySessonId].Color.Equals(MyColor);
    }

    private void Start()
    {
        //이게 내 유저라면 이벤트 실행
        if (IsClient)
        {
            UIManager.Get<UIMinigameDart>().ShowForcePower();
            orderEvent.OnAimEvent += SetAim;
            orderEvent.OnShootEvent += PressKey;
        }
    }

    #region SetProperties
    /// <summary>
    /// 각 속성의 최소 최대 결정
    /// </summary>
    public void SetAimRange(float min, float max)
    {
        minAim = min;
        maxAim = max;
    }
    public void SetForceRange(float min, float max)
    {
        minForce = min;
        maxForce = max;
    }
    public void SetPlayerIndex(int idx)
    {
        MyColor = idx;
    }
    #endregion

    private void FixedUpdate()
    {
        //키를 누르는 동안
        if(actionPhase == 1)
        {
            SetForce();
        }

        //각도를 조절
        if (GetAim != Vector2.zero)
        {
            CurAim += new Vector3(GetAim.y, GetAim.x);
        }

        transform.rotation = Quaternion.Euler(CurAim);

        Debug.DrawRay(transform.position, -transform.forward * 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rgdby.useGravity = false;
        rgdby.constraints = RigidbodyConstraints.FreezeAll;

        //다트가 판을 따라가게
        transform.SetParent(collision.transform);

        //collision.transform으로 불러오기
        MyDistance = Vector3.Distance(collision.transform.position, transform.position);

        if (IsClient)
        {
            orderEvent.OnAimEvent -= SetAim;
            orderEvent.OnShootEvent -= PressKey;
            UIManager.Get<UIMinigameDart>().HideForcePower();
        }
        ThrowToServer();
    }

    private void OnTriggerEnter(Collider other)
    {
        //무효처리
        MissDart();

        MinigameManager.Instance.GetMiniGame<GameDart>().NextDart();
    }

    /// <summary>
    /// 각도 조절
    /// </summary>
    private void SetAim(Vector2 direction)
    {
        GetAim = direction;
        GamePacket packet = new();
        packet.DartSyncRequest = new()
        {
            SessionId = MinigameManager.Instance.mySessonId,
            Angle = SocketManager.ToVector(GetAim)
        };

        SocketManager.Instance.OnSend(packet);
    }

    /// <summary>
    /// 힘 조절
    /// </summary>
    private void SetForce()
    {
        float speed = 1f;
        if (isIncrease) CurForce += Time.deltaTime * speed;
        else CurForce -= Time.deltaTime * speed;

        //변경된 Force를 UI에 전달
        UIManager.Get<UIMinigameDart>().ChangeForcePower(CurForce);
    }

    /// <summary>
    /// 키를 누르고 있으면
    /// </summary>
    /// <param name="press">InputValue</param>
    private void PressKey(bool press)
    {
        if(press)
        {
            actionPhase = 1;
        }
        if(actionPhase == 1)
        {
            if (!press)
            {
                NowShoot();
                actionPhase = 0;
            }
        }
    }

    /// <summary>
    /// 발사
    /// </summary>
    private void NowShoot()
    {
        rgdby.useGravity = true;
        rgdby.AddForce(-transform.forward * CurForce, ForceMode.Impulse);
        ThrowToServer();
    }

    /// <summary>
    /// 상대방이 쐈을때
    /// </summary>
    public void AnotherShoot(float force)
    {
        rgdby.useGravity = true;
        rgdby.AddForce(-transform.forward * force, ForceMode.Impulse);
        ThrowToServer();
    }

    /// <summary>
    /// 다트 빗나감
    /// </summary>
    private void MissDart()
    {
        rgdby.useGravity = false;
        rgdby.velocity = Vector3.zero;

        transform.localPosition = Vector3.zero;

        CurAim = Vector3.zero;
        CurForce = 2f;

        gameObject.SetActive(false);
        MyDistance = 10;    //랭크에서 빠지는 걸로
        MyRank = MinigameManager.Instance.GetMiniGame<GameDart>().MissRank;

    }

    /// <summary>
    /// 해당 데이터를 서버에 전송
    /// </summary>
    public void ThrowToServer()
    {
        GamePacket packet = new();
        var data = packet.DartGameThrowRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
            Distance = MyDistance,
            Angle = SocketManager.ToVector(CurAim),
            Location = SocketManager.ToVector(transform.localPosition),
            Power = CurForce
        };
        SocketManager.Instance.OnSend(packet);
    }
}
