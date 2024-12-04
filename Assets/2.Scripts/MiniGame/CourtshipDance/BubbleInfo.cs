public class BubbleInfo
{
    public int Color { get; private set; }
    public float Rotation { get; private set; }

    public BubbleInfo(float rotation, int color = -1)
    {
        this.Rotation = rotation;
        this.Color = color;
    }
    
    public void SetColor(int color)
    {
        this.Color = color;
    }
    public void SetRotation(float rotation)
    {
        this.Rotation = rotation;
    }
}
