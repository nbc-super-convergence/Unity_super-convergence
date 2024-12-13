using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardUI : UIBase
{
    [SerializeField] private List<BoardTokenUI> tokens;
    public event Action OnRefresh;

    private void Start()
    {
        var list = BoardManager.Instance.playerTokenHandlers;

        for (int i = 0; i < list.Count; i++)
        {
            var data = list[i].data;
            tokens[i].SetPlayer(data);
            tokens[i].SetActive(true);
        }

        Refresh();
    }

    public override void Opened(object[] param)
    {
        base.Opened(param);
        Refresh();
    }

    public override void Closed(object[] param)
    {
        base.Closed(param);

        ExitPlayer();
    }

    public void Refresh()
    {
        OnRefresh?.Invoke();
    }

    private void ExitPlayer()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            var list = BoardManager.Instance.playerTokenHandlers;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].data.userInfo.Order == -1)
                {
                    tokens[i].SetActive(false);
                    tokens[i].ExitPlayer();
                }
            }
        }
    }
}
