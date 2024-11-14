using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IceSlidingBase : MonoBehaviour
{
    //받을 컴포넌트
    private Rigidbody _rigidbody;
    private IceSlidingDamage _damage;

    //입력 속성
    private Vector3 _moveControll = Vector3.zero;

    //물리적 속성
    private Vector3 _mainDirection = Vector3.zero;  //이동할 좌표
    [SerializeField] private float _slideSpeed = 10f; //이동 속도
    [SerializeField] private float _slideFactor = 1f; //미끄러짐의 감속 비율
    private Vector3 _slideDirector = Vector3.zero;  //미끄러짐 감속의 좌표

    //플레이어 속성
    public int PlayerHP { get; private set; } = 100;

    private void Awake()
    {
        //컴포넌트 얻기
        _rigidbody = GetComponent<Rigidbody>();
        _damage = GetComponent<IceSlidingDamage>();
    }

    private void Start()
    {
        _rigidbody.freezeRotation = true; // Rigidbody의 회전을 잠가 직접 회전 제어
    }

    //물리적 처리
    private void FixedUpdate()
    {
        ApplyMove(_moveControll);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            //데미지 처리
        }
    }

    //물리적 적용
    /// <summary>
    /// 받아온 입력값 대로 이동
    /// </summary>
    /// <param name="dir">moveControll에서 적용</param>
    private void ApplyMove(Vector3 dir)
    {
        //1인칭 시점에서 이동
        _mainDirection = transform.forward * dir.z + transform.right * dir.x;

        //미끄러짐 효과
        _rigidbody.AddForce(_mainDirection * _slideSpeed, ForceMode.Acceleration);

        // 감속 효과
        Vector3 velocity = _rigidbody.velocity;
        velocity.x *= _slideFactor;
        velocity.z *= _slideFactor;
        _rigidbody.velocity = velocity;
    }

    //Todo : 장애물이랑 데미지 바닥 이랑 충돌하면 데미지 감소

    /// <summary>
    /// 입력을 받아 _moveControll에 적용
    /// </summary>
    /// <param name="dir">IceSlidingController의 InputControll에서 받음</param>
    public void InputMove(Vector3 dir)
    {
        _moveControll = dir;
    }
    
}
