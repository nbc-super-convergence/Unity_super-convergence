using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BoardTokenUI : UIBase
{
    public TMP_Text nickname;
    //public TMP_Text trophy;
    public TMP_Text coin;

    private BoardTokenData data;

    public void SetPlayer(BoardTokenData data)
    {
        this.data = data;
        nickname.text = data.userInfo.Nickname;
        UIManager.Get<BoardUI>().OnRefresh += Refresh;
    }

    public void Refresh()
    {
        //trophy.text = data.trophyAmount.ToString();
        coin.text = data.keyAmount.ToString();
    }

    public void ExitPlayer()
    {
        UIManager.Get<BoardUI>().OnRefresh -= Refresh;
    }
}
