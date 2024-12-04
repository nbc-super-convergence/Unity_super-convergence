using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderDart : MonoBehaviour
{
    private Rigidbody rgdby;

    //발사 준비상태
    private enum ShootingPhase { Aim, Force, Ready };  
    private ShootingPhase phase = ShootingPhase.Aim;

    private bool isIncrease = true; //증감 여부

    private float curAim = 0f;
    private float minAim, maxAim;
    public float CurAim
    {
        get => curAim;
        set
        {
            curAim = Mathf.Clamp(value, minAim, maxAim);
            if (curAim <= minAim)
                isIncrease = true;
            if (curAim >= maxAim)
                isIncrease = false;
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

    //나갈 각도
    private Vector3 dartRot = Vector3.back;

    public DiceGameData DiceGameData { get; private set; }

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

    private void FixedUpdate()
    {
        switch(phase)
        {
            case ShootingPhase.Aim:
                SetAim();
                break;
            case ShootingPhase.Force:
                SetForce();
                break;
            default:
                return;
        }

        transform.rotation = Quaternion.Euler(CurAim, 0, 0);

        //Debug.DrawRay(transform.position, transform.position + transform.forward);
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
            isIncrease = true;
            switch(phase)
            {
                case ShootingPhase.Aim:
                    phase = ShootingPhase.Force;
                    break;
                case ShootingPhase.Force:
                    phase = ShootingPhase.Ready;
                    NowShoot();
                    break;
            }
        }
    }

    /// <summary>
    /// 각도 조절
    /// </summary>
    private void SetAim()
    {
        float speed = 5f;

        if (isIncrease) CurAim += Time.deltaTime * speed;
        else CurAim -= Time.deltaTime * speed;

        //벡터 각도 조절
        dartRot.z = Mathf.Sin(CurAim - 90f);
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

        CurAim = 0f;
        //CurForce = 2.5f;

        phase = ShootingPhase.Aim;
    }


    public void SetDartDistance(float dist)
    {
        float distance = dist;
        //DiceGameData.Value = dist;    //float형 distance가 필요한데 일단 없어서 이렇게...
    }

    public void SetDartRank(int rank)
    {
        DiceGameData.Rank = rank;
    }
}
