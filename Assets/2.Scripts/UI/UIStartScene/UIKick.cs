using UnityEngine;
using UnityEngine.UI;

public class UIKick : UIBase
{
    [SerializeField] private Button buttonKick;
    public int targetPlayerId;
    
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y);
    }

    public void SetPlayerId(int playerId)
    {
        this.targetPlayerId = playerId;
    }

    public void Kick()
    {
        // 서버에 강퇴를 요청.
        // 패킷전송
    }
}