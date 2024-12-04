public class UserInfo
{
    //public UserData userData = new();   // 세션아이디와 닉네임
    //색깔, 순서, 닉네임
    public string SessionId { get; private set; }
    public string Nickname { get; private set; }
    public int Color { get; private set; }
    public int Order { get; private set; }



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


    public UserData ToUserData()
    {
        UserData userData = new UserData()
        {
            SessionId = SessionId,
            Nickname = Nickname,
        };
        return userData;
    }

    public void SetUserData(UserData userData)
    {
        SessionId = userData.SessionId;
        Nickname = userData.Nickname;
    }

    public void SetSessionId(string sessionId)
    {
        this.SessionId = sessionId;
    }
    public void SetNickname(string nickname)
    {
        this.Nickname = nickname; 
    }

    public void SetUserData(string sessionId, string nickname)
    {
        this.SessionId= sessionId;
        this.Nickname = nickname;
    }

    public void SetColor(int color)
    {
        this.Color = color;
    }

    public void SetOrder(int order)
    { 
        this.Order = order; 
    }
}