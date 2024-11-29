using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectOrderDart : MonoBehaviour
{
    [SerializeField] SelectOrderUI selectUI;
    private Rigidbody rgdby;

    //발사 준비상태
    private enum ShootingPhase { Aim, Force, Ready };  
    private ShootingPhase phase = ShootingPhase.Aim;

    private bool isIncrease = true; //증감 여부

    //각도 조절
    private float _aim = 90f;
    public float ShootingAim
    {
        get => _aim;
        set
        {
            float min = 90f, max = 100f;
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
            float min = 2.0f, max = 3.5f;
            _force = Mathf.Clamp(value, min, max);

            if(_force <= min)
                isIncrease = true;
            else if(_force >= max)
                isIncrease = false;
        }
    }

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        rgdby.useGravity = false;
        rgdby.freezeRotation = true;
        rgdby.constraints = RigidbodyConstraints.FreezePosition;
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

    private void SetAim()
    {
        float speed = 2.5f;
        if (isIncrease)
            ShootingAim += Time.deltaTime * speed;
        else
            ShootingAim -= Time.deltaTime * speed;

        selectUI.GetAim(ShootingAim);
    }

    private void SetForce()
    {
        float speed = 1f;
        if (isIncrease)
            ShootingForce += Time.deltaTime * speed;
        else
            ShootingForce -= Time.deltaTime * speed;

        selectUI.GetForce(ShootingForce);
    }

    private void NowShoot()
    {
        rgdby.useGravity = true;
        rgdby.AddForce(Vector3.back * ShootingForce, ForceMode.Impulse);
    }
}
