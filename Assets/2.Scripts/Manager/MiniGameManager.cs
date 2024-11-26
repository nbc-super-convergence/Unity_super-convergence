using System;
using UnityEngine;

public enum eGameType
{//���� Ŭ������� ��ġ�ؾ� ��.
    GameIceSlider,
    GameBombDelivery,
}

public class MiniGameManager : Singleton<MiniGameManager>
{
    eGameType type;
    private IGame curMiniGame;
    private MiniGameData gameData;

    [SerializeField] private MiniPlayer[] miniPlayers;
    
    #region Properties
    public T GetMiniGame<T>() where T : IGame
    {
        type = (eGameType)Enum.Parse(typeof(eGameType), nameof(T));
        return (T)curMiniGame;
    }

    public MiniPlayer GetMiniPlayer(int playerID)
    {
        int idx = playerID switch
        {
            1 => 0,
            2 => 1,
            3 => 2,
            4 => 3,
            _ => -1
        };

        if (idx == -1) return null;
        else return miniPlayers[idx];
    }
    #endregion

    #region Minigame �ʱ�ȭ
    /// <summary>
    /// �������� ���� �̴ϰ��� ����.
    /// </summary>
    /// <typeparam name="T">IGame�� �ڽ� Ŭ����</typeparam>
    public T SetMiniGame<T>() where T : IGame, new()
    {
        curMiniGame = new T();
        Init(nameof(T));
        return (T)curMiniGame;
    }

    private void Init(string gameType)
    {
        SetData(gameType);
        SetMap(gameType);
        SetBGM(gameType);
    }

    private void SetData(string gameType)
    {

    }

    //�̴ϰ��� �� ���� - kh
    private void SetMap(string gameType)
    {

    }

    //����� ���� - kh
    private void SetBGM(string gameType)
    {

    }
    #endregion

    #region Update
    //���� ���� ���� - kh
    public void ChangeScore(int _score)
    {

    }

    //���� �ð� ���� - kh
    public void ChangeTime(float _time)
    {

    }
    #endregion

    public virtual void GameStop()
    {

    }

    public virtual void GameEnd()
    {

    }
}
