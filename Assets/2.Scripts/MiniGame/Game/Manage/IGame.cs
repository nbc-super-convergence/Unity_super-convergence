using System.Collections.Generic;

public interface IGame
{
    public void Init();
    public void GameStart();
    public void GameEnd(Dictionary<string, int> ranks);
}