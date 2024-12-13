using System.Collections.Generic;

public interface IGame
{
    public void Init(params object[] param);
    public void GameStart(params object[] param);
    public async void GameEnd(List<(int Rank, string SessionId)> ranks, long boardTime)
    {
        foreach (var mini in MinigameManager.Instance.miniTokens)
        {
            mini.gameObject.SetActive(false);
        }

        await UIManager.Show<UIMinigameResult>(ranks, boardTime);
    }
    public void DisableUI();
}