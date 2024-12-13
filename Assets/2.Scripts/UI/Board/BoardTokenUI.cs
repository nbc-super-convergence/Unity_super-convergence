using UnityEngine;
using TMPro;
using System.Collections;

public class BoardTokenUI : UIBase
{
    public TMP_Text nickname;
    //public TMP_Text trophy;
    public TMP_Text coin;

    private BoardTokenData data;

    public GameObject eventUI;

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

    public void Event()
    {
        
    }

    public IEnumerator OnEvent(int coin)
    {
        eventUI.SetActive(true);

        
        yield return new WaitForSeconds(1.0f);

        eventUI.SetActive(false);
    }

    public void ExitPlayer()
    {
        UIManager.Get<BoardUI>().OnRefresh -= Refresh;
    }
}
