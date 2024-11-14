using System.Collections;
using UnityEngine;

public class IceSlidingDamage : MonoBehaviour
{
    //���� ������Ʈ
    private IceSlidingBase _iceBase;

    public int PlayerHP { get; private set; } = 100;

    private WaitForSeconds _damageDelay; //�����̸� �ֱ� ���� ���
    private float _damageSecond = 1f;    //������ ��� �ð�

    public bool NowStun { get; private set; }   //������ Ŭ���� �ȿ����� ����

    private void Awake()
    {
        _iceBase = GetComponent<IceSlidingBase>();

        _damageDelay = new WaitForSeconds(_damageSecond);
    }

    /// <summary>
    /// ������ ȿ��
    /// </summary>
    /// <param name="dmg"></param>
    public IEnumerator DamageDelay(int dmg)
    {
        while (_iceBase.IsDamage)
        {
            PlayerHP = Mathf.Max(0, PlayerHP - dmg);
            yield return _damageDelay;
            Debug.Log(PlayerHP);
        }
    }

    /// <summary>
    /// ���� ȿ��
    /// </summary>
    /// <param name="sec">�ڷ�ƾ���� �Ѱ� ������</param>
    public void GetStun(float sec)
    {
        //�̵� �� ȸ�� ����
        NowStun = true;
        StartCoroutine(StunDelay(sec));
    }
    private IEnumerator StunDelay(float sec)
    {
        //Debug.Log("���� ����");
        yield return new WaitForSeconds(sec);
        //Debug.Log("���� ��");
        NowStun = false;
    }
}
