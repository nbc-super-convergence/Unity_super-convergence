using System.Collections.Generic;

public interface IGame
{
    public void Init(params object[] param);
    public void GameStart();
    public void GameEnd(Dictionary<string, int> ranks);
}