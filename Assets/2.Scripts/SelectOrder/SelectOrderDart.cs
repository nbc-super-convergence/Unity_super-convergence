using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderDart : MonoBehaviour
{
    private SelectOrderEvent orderEvent;
    private Rigidbody rgdby;

    //발사 준비상태
    private enum ShootingPhase { Ready };  
    private ShootingPhase phase = ShootingPhase.Ready;

    //서버 전송 데이터
    private DiceGameData diceData;
    public DiceGameData DiceGameData
    {
        get => diceData;
        private set
        {
            diceData = value;
        }
    }

    private bool isIncrease = true; //증감 여부

    private float curAim = 0f;
    private float curAimX = 0f;
    private float minAim, maxAim;
    private Vector3 aimVector;
    public Vector3 CurAim
    {
        get => aimVector;
        set
        {
            aimVector.x = Mathf.Clamp(value.x, minAim, maxAim);
            aimVector.y = Mathf.Clamp(value.y, minAim, maxAim);
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

    //나갈 각도
    private Vector3 dartRot = Vector3.back;

    private void Awake()
    {
        rgdby = GetComponent<Rigidbody>();
        orderEvent = GetComponent<SelectOrderEvent>();

        DiceGameData = new();
    }

    private void Start()
    {
        orderEvent.OnAimEvent += SetAim;
        orderEvent.OnShootEvent += NowShoot;

        minAim = SelectOrderManager.Instance.minAim;
        maxAim = SelectOrderManager.Instance.maxAim;
        minForce = SelectOrderManager.Instance.minForce;
        maxForce = SelectOrderManager.Instance.maxForce;
    }

    private void FixedUpdate()
    {
        switch(phase)
        {
            case ShootingPhase.Ready:
                SetForce();
                break;
        }

        //각도를 조절
        if(GetAim != Vector2.zero)
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

        orderEvent.OnAimEvent -= SetAim;
    }

    private void OnTriggerEnter(Collider other)
    {
        //빗나가면 다시
        ResetDart();
    }

    //입력 받을 때
    public void OnShooting(InputAction.CallbackContext context)
    {
        if (InputActionPhase.Started == context.phase)
        {
        }
    }

    /// <summary>
    /// 각도 조절
    /// </summary>
    private void SetAim(Vector2 direction)
    {
        GetAim = direction;
    }

    /// <summary>
    /// 힘 조절
    /// </summary>
    private void SetForce()
    {
        float speed = 1f;
        if (isIncrease) CurForce += Time.deltaTime * speed;
        else CurForce -= Time.deltaTime * speed;
    }

    /// <summary>
    /// 발사
    /// </summary>
    private void NowShoot()
    {
        rgdby.useGravity = true;
        rgdby.AddForce(-transform.forward * CurForce, ForceMode.Impulse);
    }

    /// <summary>
    /// 다트 초기화
    /// </summary>
    private void ResetDart()
    {
        rgdby.useGravity = false;
        rgdby.velocity = Vector3.zero;

        transform.localPosition = Vector3.zero;

        CurAim = Vector3.zero;
        CurForce = 2f;

        phase = ShootingPhase.Ready;
    }

    /// <summary>
    /// 다트 표적 벡터
    /// </summary>
    /// <returns>WorldToScreenPoint</returns>
    public Vector3 TargetPosition()
    {
        return Camera.main.WorldToScreenPoint(-transform.forward);
    }

    /// <summary>
    /// 해당 데이터를 서버에 전송
    /// </summary>
    public void SendServer()
    {
        GamePacket packet = new GamePacket();
        var data = packet.DiceGameRequest = new()
        {
            Distance = MyDistance,
            SessionId = gameObject.name
            //SessionId = GameManager.Instance.myInfo.SessionId
        };
        SocketManager.Instance.OnSend(packet);
    }
}
