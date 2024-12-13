public class CourtshipDanceData : IGameData
{
    public float totalTime;

    public int boardAmount;
    public int individualBoardAmount;
    public int minBubbleCount;
    public int maxBubbleCount;

    public void Init()
    {
        totalTime = 120f;

        boardAmount = 12;
        individualBoardAmount = 12;
        minBubbleCount = 3;
        maxBubbleCount = 13;
    }
}