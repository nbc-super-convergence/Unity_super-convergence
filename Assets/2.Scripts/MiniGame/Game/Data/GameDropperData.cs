public class GameDropperData : IGameData
{
    public int phase;
    public int curSlot;

    public void Init()
    {
        phase = 0;
        curSlot = 4;
    }
}