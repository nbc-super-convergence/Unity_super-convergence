using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderDart : MonoBehaviour
{
    private Rigidbody rgdby;

    //발사 준비상태
    private enum ShootingPhase { Ready };  
    private ShootingPhase phase = ShootingPhase.Ready;

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

    public float MyDistance { get; set; } = 30f;
    public int MyRank { get; set; } = 0;

    //나갈 각도
    private Vector3 dartRot = Vector3.back;

    private DiceGameData _diceData;
    public DiceGameData DiceGameData 
    { 
        get => _diceData;
        private set
        {

        }
    }

    private void Awake()
    {
        rgdby = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        minAim = SelectOrderManager.Instance.minAim;
        maxAim = SelectOrderManager.Instance.maxAim;
        minForce = SelectOrderManager.Instance.minForce;
        maxForce = SelectOrderManager.Instance.maxForce;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interaction();
        }
    }

    private void FixedUpdate()
    {
        switch(phase)
        {
            case ShootingPhase.Ready:
                SetAim();
                SetForce();
                break;
        }

        //각도를 조절
        transform.rotation = Quaternion.Euler(CurAim);

        Debug.DrawRay(transform.position, -transform.forward * 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rgdby.useGravity = false;
        rgdby.constraints = RigidbodyConstraints.FreezeAll;

        //다트가 판을 따라가게
        transform.SetParent(collision.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        //다트가 빗나가면
        ResetDart();
    }

    //입력 받을 때
    public void OnShooting(InputAction.CallbackContext context)
    {
        if (InputActionPhase.Started == context.phase)
        {
        }
    }

    private void Interaction()
    {
        isIncrease = true;
        switch (phase)
        {
            case ShootingPhase.Ready:
                NowShoot();
                break;
        }
    }

    /// <summary>
    /// 각도 조절
    /// </summary>
    private void SetAim()
    {
        //입력을 WASD로 받아 상하좌우 조준
        float horizon = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 aim = new Vector3(vertical, horizon, 0);
        CurAim += aim;

        //if (isIncrease) CurAim += Time.deltaTime * speed;
        //else CurAim -= Time.deltaTime * speed;

        //벡터 각도 조절
        //dartRot.z = Mathf.Sin(CurAim - 90f);
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
}
