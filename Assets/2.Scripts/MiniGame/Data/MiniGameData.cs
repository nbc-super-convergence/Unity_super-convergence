using System;
using System.Collections;
using UnityEngine;

public class MiniGameData
{
    public void InitGameData(eGameType type)
    {
        switch (type)
        {
            case eGameType.GameIceSlider:
                TotalTime = 120f;
                SetAllPlayerHP(10);
                break;
            case eGameType.GameBombDelivery:
                break;
        }
    }

    #region TotalTime
    public float TotalTime { get; private set; }
    public float CurrentTime { get; private set; }
    public IEnumerator DecreaseTimeCoroutine(float time, Action TimeEnd)
    {
        CurrentTime = time;
        while (CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;
            yield return null;
        }
        TimeEnd?.Invoke();
    }
    #endregion

    #region PlayerHP
    public int[] playerHps { get; private set; } = new int[4];
    public void SetAllPlayerHP(int hp)
    {
        for (int i = 0; i < playerHps.Length; i++)
        {
            playerHps[i] = hp;
        }
        
    }
    public void GivePlayerDamage(eGameType type, string sessionId, int dmg)
    {
        int idx = GameManager.Instance.SessionDic[sessionId];
        switch (type)
        {
            case eGameType.GameIceSlider:
                playerHps[idx] -= dmg;
                break;
            case eGameType.GameBombDelivery:
                break;
        }
    }
    #endregion
}
