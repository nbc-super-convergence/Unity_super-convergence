using System.Collections;
using UnityEngine;

public class IceSlidingDamage : MonoBehaviour
{
    //연결 컴포넌트
    private MiniPlayer player;
    private PlayerHealth iceHealth;

    private WaitForSeconds _damageDelay; //딜레이를 주기 위한 대기
    private float _damageSecond = 1f;    //데미지 대기 시간

    public bool NowStun { get; private set; }   //데미지 클래스 안에서만 설정
    public bool NowAlive { get; private set; }

    private void Awake()
    {
        player = GetComponent<MiniPlayer>();
        iceHealth = GetComponent<PlayerHealth>();

        _damageDelay = new WaitForSeconds(_damageSecond);
    }

    /// <summary>
    /// 데미지 효과
    /// </summary>
    /// <param name="dmg"></param>
    public IEnumerator DamageDelay(int dmg)
    {
        //while (_iceBase.IsDamage)
        {
            iceHealth.DecreaseHP(dmg);
            NowAlive = iceHealth.PlayerHP > 0;
            yield return _damageDelay;
            //Debug.Log(PlayerHP);
        }
    }

    /// <summary>
    /// 스턴 효과
    /// </summary>
    /// <param name="sec">코루틴으로 넘겨 딜레이</param>
    public void GetStun(float sec)
    {
        //이동 및 회전 금지
        NowStun = true;
        StartCoroutine(StunDelay(sec));
    }
    private IEnumerator StunDelay(float sec)
    {
        //Debug.Log("스턴 시작");
        yield return new WaitForSeconds(sec);
        //Debug.Log("스턴 끝");
        NowStun = false;
    }
}
