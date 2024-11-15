using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(IceSlidingDamage))]
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

    public bool CheckAlive { get; private set; }    //현재 살아남아 있는지
    public bool CheckStun { get; private set; }     //현재 스턴에 걸렸는지
    public bool IsDamage { get; private set; }  //현재 데미지에 걸렸는지

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

    //로직 처리
    private void Update()
    {
        //상태 검사
        CheckAlive = _damage.NowAlive;
        CheckStun = _damage.NowStun;
    }

    //물리적 처리
    private void FixedUpdate()
    {
        //if (_damage.PlayerHP > 0)   //HP가 남으면
            ApplyMove(_moveControll);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //특정 위치 안에 있으면 데미지 감소
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            IsDamage = true;    //데미지 발생
            if(collision.gameObject.name.Equals("20001"))   //이거는 나중에 ID로 받을 예정임
            {
                //튕겨나가기
                BounceOut(collision);

                //0.2초간 스턴
                _damage.GetStun(2f);
            }

            //데미지 처리
            StartCoroutine(_damage.DamageDelay(1));
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            IsDamage = false;
            StopCoroutine(_damage.DamageDelay(1));  //코루틴 끝내기
        }
    }

    //물리적 적용
    /// <summary>
    /// 받아온 입력값 대로 이동
    /// </summary>
    /// <param name="dir">moveControll에서 적용</param>
    private void ApplyMove(Vector3 dir)
    {
        //이동
        _mainDirection = transform.forward * dir.z + transform.right * dir.x;

        //미끄러짐 효과
        _rigidbody.AddForce(_mainDirection * _slideSpeed, ForceMode.Acceleration);

        // 감속 효과
        Vector3 velocity = _rigidbody.velocity;
        velocity.x *= _slideFactor;
        velocity.z *= _slideFactor;
        _rigidbody.velocity = velocity;
    }

    /// <summary>
    /// 입력을 받아 _moveControll에 적용
    /// </summary>
    /// <param name="dir">IceSlidingController의 InputControll에서 받음</param>
    public void InputMove(Vector3 dir)
    {
        _moveControll = dir;
    }

    /// <summary>
    /// 튕겨나가는 효과
    /// </summary>
    /// <param name="collision">OnCollision에서</param>
    private void BounceOut(Collision collision)
    {
        float bounceForce = 5f; // 튕겨나가는 힘의 세기  (60초 지나면 강도가 강해지게)
                                // 충돌 지점의 법선 벡터 가져오기 (첫 번째 접촉 지점의 법선)
        Vector3 collisionNormal = collision.GetContact(0).normal;

        // 상하좌우 방향 중 가장 가까운 방향을 bounceDirection으로 설정
        Vector3 bounceDirection = new Vector3(
            Mathf.Round(collisionNormal.x),
            Mathf.Round(collisionNormal.y),
            Mathf.Round(collisionNormal.z)
        );

        // 방향이 설정되었으면 튕겨나가는 힘 적용
        if (bounceDirection != Vector3.zero)
        {
            _rigidbody.velocity = Vector3.zero; // 기존 속도 초기화
            _rigidbody.AddForce(bounceDirection * bounceForce, ForceMode.VelocityChange);
        }
    }
}
