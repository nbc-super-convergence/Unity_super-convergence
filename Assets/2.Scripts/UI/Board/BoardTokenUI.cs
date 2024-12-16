using UnityEngine;
using TMPro;
using System.Collections;
using TMPro;

public class BoardTokenUI : UIBase
{
    public TMP_Text nickname;
    //public TMP_Text trophy;
    public TMP_Text coin;

    private BoardTokenData data;

    public  TMP_Text eventUI;

    public void SetPlayer(BoardTokenData data)
    {
        this.data = data;
        nickname.text = data.userInfo.Nickname;
        UIManager.Get<BoardUI>().OnRefresh += Refresh;
    }

    public void Refresh()
    {
        //trophy.text = data.trophyAmount.ToString();
        coin.text = data.coin.ToString();
    }

    public void Event(int coin,bool isMinus)
    {
        eventUI.text = "";

        if (!isMinus) eventUI.text = "+";

        eventUI.text += coin.ToString();
        StartCoroutine(OnEvent());
    }

    public IEnumerator OnEvent()
    {
        eventUI.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);

        eventUI.gameObject.SetActive(false);
    }

    public void ExitPlayer()
    {
        UIManager.Get<BoardUI>().OnRefresh -= Refresh;
    }
}
