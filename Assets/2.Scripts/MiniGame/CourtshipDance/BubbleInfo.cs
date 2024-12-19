public class BubbleInfo
{
    public int Color { get; private set; }
    public float Rotation { get; private set; }
    public string SessionId { get; private set; }


    public BubbleInfo() { }

    public BubbleInfo(float rotation, int color = -1)
    {
        this.Rotation = rotation;
        this.Color = color;
    }

    public BubbleInfo(BubbleInfo other)
    {
        this.Color = other.Color;
        this.Rotation = other.Rotation;
        this.SessionId = other.SessionId;
    }

    public BubbleInfo Clone()
    {
        return new BubbleInfo(this);
    }

    public void SetColor(int color)
    {
        this.Color = color;
        this.SessionId = GameManager.Instance.FindSessionIdByColor(color);      // TODO:: 간결한 방법으로 바꾸기.
    }

    public void SetColor(string sessionId)
    {
        this.Color = MinigameManager.Instance.GetMiniToken(sessionId).MyColor;
    }

    public void SetRotation(float rotation)
    {
        this.Rotation = rotation;
    }

    public void SetSessionId(string sessionId)
    { 
        this.SessionId = sessionId; 
    }    
}