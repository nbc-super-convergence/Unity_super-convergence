using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BoardResultUI : UIBase
{
    //public TMP_Text text;
    //List<string> comment = new();
    [SerializeField] BoardResultRankUI prefab;
    [SerializeField] Transform layout;
    [SerializeField] TMP_Text timeText;
    private List<BoardResultRankUI> result = new();
    private bool isEnd;
    private float timer;

    public override void Opened(object[] param)
    {
        timer = 15f;

        int length = MinigameManager.Instance.miniTokens.Length;

        for(int i = 0; i < length; i++)
            MinigameManager.Instance.miniTokens[i].DisableMiniToken();

        for (int i = 0; i < BoardManager.Instance.playerTokenHandlers.Count; i++)
            BoardManager.Instance.playerTokenHandlers[i].gameObject.SetActive(false);

        //base.Opened(param);
        //result = (List<IGameResult>)param[0];

        //SetComment();

        //StartCoroutine(Result());

        var list = BoardManager.Instance.playerTokenHandlers;

        list.Sort((a,b) => b.data.coin.CompareTo(a.data.coin));

        for (int i = 0; i < list.Count; i++)
        {
            var g = Instantiate(prefab,layout);

            g.rank.text = (i + 1).ToString();
            g.id.text = list[i].data.userInfo.Nickname;
            //g.trophy.text = list[i].data.trophyAmount.ToString();
            g.coin.text = list[i].data.coin.ToString();

            g.gameObject.SetActive(false);
            result.Add(g);
        }

        StartCoroutine(GameEnd());
    }

    private IEnumerator GameEnd()
    {
        for(int i = 0; i < result.Count; i++)
        {
            result[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        isEnd = true;
    }

    private void Update()
    {
        if(isEnd)
        {
            timer -= Time.deltaTime;

            timeText.text = timer.ToString("F0") + "초 후에 방으로 돌아갑니다.";
            if(timer < 0.0f) OnClick();
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

    public void OnClick()
    {
        GameManager.isGameStart = false;
        GamePacket packet = new();
        packet.BackToTheRoomRequest = new()
        {
            SessionId = GameManager.Instance.myInfo.SessionId,
        };

        SocketManager.Instance.OnSend(packet);
    }
}