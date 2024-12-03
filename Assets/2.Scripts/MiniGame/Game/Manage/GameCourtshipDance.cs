using System.Collections.Generic;

public class GameCourtshipDance : IGame
{
    /// <summary>
    /// MinigameManager의 SetMiniGame에서 실행함.
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
    {

    }
    public void GameEnd(Dictionary<string, int> ranks)
    {
        throw new System.NotImplementedException();
    }

    public void GameStart()
    {
        throw new System.NotImplementedException();
    }
}