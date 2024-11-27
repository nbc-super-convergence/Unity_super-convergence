using UnityEngine;
using UnityEngine.UI;

public class UIKick : UIBase
{
    [SerializeField] private Button buttonKick;
    [SerializeField] private GameObject overlay;
    public string targetPlayerId;

    public override void Opened(object[] param)
    {
        overlay.SetActive(true);
    }
    public override void Closed(object[] param)
    {
        overlay.SetActive(false);
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y);
    }

    public void SetPlayerId(string playerId)
    {
        this.targetPlayerId = playerId;
    }

    public void Kick()
    {
        // 서버에 강퇴를 요청.
        // 패킷전송

        Debug.Log($"PlayerId {targetPlayerId}을/를 서버에 추방요청함.");
        UIManager.Hide<UIKick>();
    }
}