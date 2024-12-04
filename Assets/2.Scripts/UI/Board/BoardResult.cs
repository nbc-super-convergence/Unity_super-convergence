using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class BoardResult : UIBase
{
    //public TMP_Text text;
    //List<string> comment = new();
    [SerializeField] BoardResultUI prefab;
    [SerializeField] Transform layout;

    public override async void Opened(object[] param)
    {
        //base.Opened(param);
        //result = (List<IGameResult>)param[0];

        //SetComment();

        //StartCoroutine(Result());

        var list = BoardManager.Instance.playerTokenHandlers;

        for (int i = 0; i < list.Count; i++)
        {
            var g = Instantiate(prefab,layout);

            g.id.text = list[i].data.id;
            g.trophy.text = list[i].data.trophyAmount.ToString();
            g.coin.text = list[i].data.keyAmount.ToString();
        }
    }

    //private IEnumerator Result()
    //{
    //    for(int i = 0; i < comment.Count; i++)
    //    {
    //        text.text = comment[i];

    //        yield return new WaitForSeconds(1.0f);
    //    }
    //}

    //private void SetComment()
    //{
    //    var list = BoardManager.Instance.playerTokenHandlers;

    //    comment.Add("게임이 종료되었습니다.");

    //    //if(result.Count > 0)
    //    //{
    //    //    comment.Add("결과를 발표하기전에 저희가 설정했던 비밀 임무가 있습니다.");
    //    //    comment.Add("해당 임무를 수행해주신분께 트로피를 수여하겠습니다.");
    //    //}

    //    //comment.Add("결과를 발표하겠습니다!");
    //}

    public void OnClick()
    {
        //GamePacket packet = new();
        //packet.BackToTheRoomRequest = new()
        //{
        //    //UserId = 
        //};

        //SocketManager.Instance.OnSend(packet);
    }
}