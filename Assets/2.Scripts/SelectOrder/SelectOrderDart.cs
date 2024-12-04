using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderDart : MonoBehaviour
{
    [SerializeField] private SelectOrderUI selectUI;
    private Rigidbody rgdby;

    //발사 준비상태
    private enum ShootingPhase { Aim, Force, Ready };  
    private ShootingPhase phase = ShootingPhase.Aim;

    private bool isIncrease = true; //증감 여부

    //각도 조절
    private float _aim = 0f;
    public float ShootingAim
    {
        get => _aim;
        set
        {
            float min = 0f, max = 20f;
            _aim = Mathf.Clamp(value, min, max);

            if (_aim <= min)
                isIncrease = true;
            else if (_aim >= max)
                isIncrease = false;
        }
    }

    //힘 조절
    private float _force = 2.5f;
    public float ShootingForce
    {
        get => _force;
        set
        {
            float min = 1.5f, max = 3f;
            _force = Mathf.Clamp(value, min, max);

            if(_force <= min)
                isIncrease = true;
            else if(_force >= max)
                isIncrease = false;
        }
    }

    //나갈 각도
    private Vector3 dartRot = Vector3.back;

    private void Awake()
    {
        rgdby = GetComponent<Rigidbody>();
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

        transform.rotation = Quaternion.Euler(ShootingAim, 0, 0);

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
        float speed = 3f;
        if (isIncrease)
            ShootingAim += Time.deltaTime * speed;
        else
            ShootingAim -= Time.deltaTime * speed;

        //벡터 각도 조절
        dartRot.z = Mathf.Sin(ShootingAim - 90f);

        selectUI.GetAim(ShootingAim);
    }

    /// <summary>
    /// 힘 조절
    /// </summary>
    private void SetForce()
    {
        float speed = 1f;
        if (isIncrease)
            ShootingForce += Time.deltaTime * speed;
        else
            ShootingForce -= Time.deltaTime * speed;

        selectUI.GetForce(ShootingForce);
    }

    /// <summary>
    /// 발사
    /// </summary>
    private void NowShoot()
    {
        rgdby.useGravity = true;
        rgdby.AddForce(-transform.forward * ShootingForce, ForceMode.Impulse);
    }

    /// <summary>
    /// 다트 초기화
    /// </summary>
    private void ResetDart()
    {
        rgdby.useGravity = false;
        rgdby.velocity = Vector3.zero;

        transform.localPosition = Vector3.zero;

        ShootingAim = 0f;
        ShootingForce = 2.5f;

        phase = ShootingPhase.Aim;
    }
}
