public class CourtshipDanceData : IGameData
{
    public float timerStartValue;

    public int boardAmount;
    public int individualBoardAmount;
    public int minBubbleCount;
    public int maxBubbleCount;

    public float stunDelay;

    public CourtshipDanceData()
    {
        timerStartValue = 120f;

        boardAmount = 12;
        individualBoardAmount = 14;
        minBubbleCount = 3;
        maxBubbleCount = 13;

        stunDelay = 1.6f;
    }

    public void Init()
    {
        timerStartValue = 120f;

        boardAmount = 12;
        individualBoardAmount = 14;
        minBubbleCount = 3;
        maxBubbleCount = 13;

        stunDelay = 1.6f;
    }
}