
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BoardUI : UIBase
{
    [SerializeField] private List<BoardTokenUI> tokens;
    public event Action OnRefresh;
    public GameObject myTurnUI;
    public TMP_Text turnText;

    private int turn;

    private IEnumerator Start()
    {
        turn = 0;

        yield return new WaitUntil(() => BoardManager.Instance.isInitialized);

        var list = BoardManager.Instance.playerTokenHandlers;

        for (int i = 0; i < list.Count; i++)
        {
            var data = list[i].data;
            tokens[i].SetPlayer(data);
            tokens[i].SetActive(true);
        }

        Refresh();
        UpdateTurn();
    }

    public override void Opened(object[] param)
    {
        base.Opened(param);
        SoundManager.Instance.PlayBGM(BGMType.BoardScene);
        //Refresh();
    }

    public override void Closed(object[] param)
    {
        base.Closed(param);

        //ExitPlayer();
    }

    public void Refresh()
    {
        OnRefresh?.Invoke();
    }

    public void ExitPlayer(int i)
    {
        //if(SceneManager.GetActiveScene().buildIndex == 2)
        //{
        //    var list = BoardManager.Instance.playerTokenHandlers;

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].data.userInfo.Order == -1)
        //        {
        //            tokens[i].SetActive(false);
        //            tokens[i].ExitPlayer();
        //        }
        //    }
        //}

        tokens[i].SetActive(false);
    }

    public void ShowMyTurn(bool isShow)
    {
        myTurnUI.SetActive(isShow);
    }

    public BoardTokenUI GetPlayerUI(int i)
    {
        return tokens[i];
    }

    public void UpdateTurn()
    {
        turnText.text = $"<size= 75> {++turn} </size> / 40";
    }
}
