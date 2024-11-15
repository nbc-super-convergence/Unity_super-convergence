
public class TrophyArea : IToggle
{
    private bool isTrophy = false;
    public TrophyArea()
    {
        MapControl.Instance.trophyNode.Add(this);
    }

    public void Action()
    {
        if(isTrophy)
        {
            var p = MapControl.Instance.Curplayer;
            p.data.trophyAmount++;
        }
    }

    public void Toggle()
    {
        isTrophy = !isTrophy;
    }
}
