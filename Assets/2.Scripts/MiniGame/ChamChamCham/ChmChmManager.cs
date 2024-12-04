using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChmChmManager : Singleton<ChmChmManager>
{
    public enum YourRole { Owner, Other };
    public YourRole MyRole;

    public ChmChmOwner OwnerPlayer;
    public List<ChmChmOther> OtherPlayers;

    //�������� �޾ƾ� �Ұ͵�
    //�÷��̾� Color : Owner�� ��P���� �����ؾ� �Ǽ�
    //�� ���� ȸ�� : 3ȸ�� �����ϰ� �ִµ�
    //�����ð� : ���ð� �ȿ� ���� ������ ���� ���ϱ� �ּ� 3��(?)
    public int Round = 0;
    private float curTime = 0f;
    private float matchTime = 3f;   //3�ʱ��� ���� ����

    private void Update()
    {

    }

    /// <summary>
    /// ����� �˻�
    /// </summary>
    private void CheckResult()
    {
        //�ִ� �÷��̾���� ���¸� �˻�
        foreach (ChmChmOther other in OtherPlayers)
        {
            if (other.OwnerSame(OwnerPlayer.LookDirection))
            {
                other.PlayerDead();
                OtherPlayers.Remove(other);    //������ ���ڸ����� ����
            }
        }

        Round++;

        //���� �ʰ���
        if (Round > 3)
        {
            if (OtherPlayers.Count == 0)
            {
                Debug.Log("Owner Wins");
            }
            else
            {
                Debug.Log("Others Win");
            }
        }
    }
}
