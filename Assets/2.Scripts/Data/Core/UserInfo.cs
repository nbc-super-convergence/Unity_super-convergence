public class UserInfo
{
    public UserData userData = new();
    public string sessionId { get; private set; }
    public string uuid { get; private set; }

    //sessionId : 로그인응답에서 최초 받음.
    //loginId, nickname : 로비 참가 응답에서 최초 받음.

    public UserData ToUserData()
    {
        return userData;
    }

    public void SetSessionId(string sessionId)
    {
        this.sessionId = sessionId;
    }
    public void SetUuid(string uuid)
    {
        this.uuid = uuid;
    }
}