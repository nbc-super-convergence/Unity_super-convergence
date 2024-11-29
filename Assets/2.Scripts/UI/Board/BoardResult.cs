using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class BoardResult : UIBase
{
    List<IGameResult> result;
    public TMP_Text text;
    List<string> comment = new();

    public override void Opened(object[] param)
    {
        base.Opened(param);
        result = (List<IGameResult>)param[0];

        SetComment();

        StartCoroutine(Result());
    }

    private IEnumerator Result()
    {
        for(int i = 0; i < comment.Count; i++)
        {
            text.text = comment[i];

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SetComment()
    {
        var list = BoardManager.Instance.playerTokenHandlers;

        comment.Add("������ ����Ǿ����ϴ�.");

        if(result.Count > 0)
        {
            comment.Add("����� ��ǥ�ϱ����� ���� �����ߴ� ��� �ӹ��� �ֽ��ϴ�.");
            comment.Add("�ش� �ӹ��� �������ֽźв� Ʈ���Ǹ� �����ϰڽ��ϴ�.");

            for (int i = 0; i < result.Count; i++)
            {
                var res = result[i].Result();

                if (res.Count == 0) continue;

                for(int j = 0; j < result.Count; j++)
                {
                    list[i].data.trophyAmount += 1;
                }
            }
        }

        comment.Add("����� ��ǥ�ϰڽ��ϴ�!");

    }
}