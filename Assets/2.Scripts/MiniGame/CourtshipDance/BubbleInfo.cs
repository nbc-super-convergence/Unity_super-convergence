public class BubbleInfo
{
    public int Color { get; private set; }
    public float Rotation { get; private set; }
    public string sessionId;

    
    public BubbleInfo(float rotation, int color = -1)
    {
        this.Rotation = rotation;
        this.Color = color;
    }
    public BubbleInfo(BubbleInfo other)
    {
        this.Color = other.Color;
        this.Rotation = other.Rotation;
        this.sessionId = other.sessionId;
    }

    public BubbleInfo Clone()
    {
        return new BubbleInfo(this);
    }

    public void SetColor(int color)
    {
        this.Color = color;
        this.sessionId = GameManager.Instance.FindSessionIdByColor(color);      // TODO:: 간결한 방법으로 바꾸기.
    }
    public void SetRotation(float rotation)
    {
        this.Rotation = rotation;
    }
    public void SetSessionId(string sessionId)
    { 
        this.sessionId = sessionId; 
    }    
}