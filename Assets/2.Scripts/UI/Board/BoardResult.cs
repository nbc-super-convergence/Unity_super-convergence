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

        comment.Add("게임이 종료되었습니다.");

        if(result.Count > 0)
        {
            comment.Add("결과를 발표하기전에 저희가 설정했던 비밀 임무가 있습니다.");
            comment.Add("해당 임무를 수행해주신분께 트로피를 수여하겠습니다.");

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

        comment.Add("결과를 발표하겠습니다!");

    }
}