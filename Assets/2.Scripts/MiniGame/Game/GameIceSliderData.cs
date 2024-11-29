public class GameIceSliderData : IGameData
{
    public float totalTime;
    public int[] playerHps = new int[4];
    public int phase;

    public void Init()
    {
        totalTime = 120f;

        for (int i = 0; i < playerHps.Length; i++)
        {
            playerHps[i] = 10;
        }

        phase = 3;
    }
}