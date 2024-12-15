public class UserInfo
{
    public string SessionId { get; private set; }
    public string Nickname { get; private set; }
    public int Color { get; private set; }
    public int Order { get; private set; } //TODO : Color와 동기화??

    //sessionId : 로그인응답에서 최초 받음.
    //loginId, nickname : 로비 참가 응답에서 최초 받음.
    public UserInfo() { }
    public UserInfo(string sessionId, string nickname, int color = -1, int order = -1)
    {
        SessionId = sessionId;
        Nickname = nickname;
        Color = color;
        Order = order;
    }

    public void SetSessionId(string sessionId)
    {
        SessionId = sessionId;
    }

    public void SetNickname(string name)
    {
        Nickname = name;
    }
}