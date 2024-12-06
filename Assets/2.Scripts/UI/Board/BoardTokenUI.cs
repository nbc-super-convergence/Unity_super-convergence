using UnityEngine;
using TMPro;

public class BoardTokenUI : UIBase
{
    public TMP_Text nickname;
    public TMP_Text trophy;
    public TMP_Text coin;

    private BoardTokenData data;

    public void SetPlayer(BoardTokenData data)
    {
        this.data = data;
    }

    public void Refresh()
    {
        trophy.text = data.trophyAmount.ToString();
        coin.text = data.keyAmount.ToString();
    }
}
