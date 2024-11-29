public class UIMinigameResult : UIBase
{
    public override void Opened(object[] param)
    {
        Rank[] ranks = (Rank[])param;
        foreach (var r in ranks)
        {
            //SessionId받아서 Rank 표시.
        }
    }
}