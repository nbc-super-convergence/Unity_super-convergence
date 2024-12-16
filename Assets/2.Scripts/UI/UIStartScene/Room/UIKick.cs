using TMPro;
using UnityEngine;

public class UIKick : UIBase
{
    [SerializeField] private TextMeshProUGUI warnUI;
    [SerializeField] private ChatSizeFitter warnBox;
    private int kickIdx;

    public override void Opened(object[] param)
    {
        if (param.Length == 2)
        {
            if (param[0] is int idx)
            {
                kickIdx = idx;
            }

            if (param[1] is string nickname)
            {
                warnUI.text = $"{nickname}님을\n퇴장시키겠습니까?";
            }
        }
    }

    public void OnYesBtn()
    {
        UIManager.Get<UIRoom>().KickUser(kickIdx);
        UIManager.Hide<UIKick>();
    }

    public void OnNOBtn()
    {
        UIManager.Hide<UIKick>();
    }
}