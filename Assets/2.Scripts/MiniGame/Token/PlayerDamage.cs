using System.Collections;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    //컴포넌트
    private Rigidbody playerRgdby;
    private PlayerHealth health;

    private WaitForSeconds damageDelay; //딜레이를 주기 위한 대기
    private float damageSecond = 1f;    //데미지 대기 시간

    public bool IsDamage { get; private set; }
    public bool IsStun { get; private set; }
    public bool IsAlive { get; private set; } = true;

    private void Awake()
    {
        playerRgdby = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();

        damageDelay = new WaitForSeconds(damageSecond);
    }

    private void Update()
    {
        IsAlive = health.PlayerHP > 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
        {
            IsDamage = true;
            SetDamage(1);
        }

        //가운데 장애물
        if (collision.gameObject.name.Equals("20001"))   //이거는 나중에 ID로 받을 예정임
        {
            //튕겨나가기
            BounceOut(collision);

            if (!IsStun)
            {
                //0.2초간 스턴
                StartCoroutine(SetStun(2f));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Damage")))
            IsDamage = false;
    }

    //데미지 속성
    /// <summary>
    /// 데미지 효과
    /// </summary>
    public void SetDamage(int dmg)
    {
        StartCoroutine(DamageDelay(dmg));
    }
    private IEnumerator DamageDelay(int dmg)
    {
        while (IsDamage)
        {
            //체력 dmg만큼 감소
            health.DecreaseHP(dmg);
            yield return damageDelay;
        }
    }

    /// <summary>
    /// 스턴 효과 ()
    /// </summary>
    /// <param name="sec"></param>
    private IEnumerator SetStun(float sec)
    {
        IsStun = true;
        //Debug.Log("스턴 시작");
        yield return new WaitForSeconds(sec);
        //Debug.Log("스턴 끝");
        IsStun = false;
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
            playerRgdby.velocity = Vector3.zero; // 기존 속도 초기화
            playerRgdby.AddForce(bounceDirection * bounceForce, ForceMode.VelocityChange);
        }
    }
}