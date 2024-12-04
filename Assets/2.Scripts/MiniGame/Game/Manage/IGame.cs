using System;
using System.Collections.Generic;

public interface IGame
{
    public void Init(params object[] param);
    public void GameStart();
    public async void GameEnd(Dictionary<string, int> ranks, long boardTime)
    {
        foreach (var mini in MinigameManager.Instance.MiniTokens)
        {
            mini.gameObject.SetActive(false);
        }

        await UIManager.Show<UIMinigameResult>(ranks, boardTime);
    }
}