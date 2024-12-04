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

    //    comment.Add("������ ����Ǿ����ϴ�.");

    //    //if(result.Count > 0)
    //    //{
    //    //    comment.Add("����� ��ǥ�ϱ����� ���� �����ߴ� ��� �ӹ��� �ֽ��ϴ�.");
    //    //    comment.Add("�ش� �ӹ��� �������ֽźв� Ʈ���Ǹ� �����ϰڽ��ϴ�.");
    //    //}

    //    //comment.Add("����� ��ǥ�ϰڽ��ϴ�!");
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