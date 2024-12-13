public class GameIceSliderData : IGameData
{
    public float totalTime;
    public int maxHP;
    public float[] playerHps = new float[4];
    public int phase;

    public void Init()
    {
        totalTime = 120f;
        maxHP = 10;

        for (int i = 0; i < playerHps.Length; i++)
        {
            playerHps[i] = maxHP;
        }

        phase = 1;
    }
}