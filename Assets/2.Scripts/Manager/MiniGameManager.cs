using System;
using UnityEngine;

public enum eGameType
{//실존 클래스명과 일치해야 함.
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

    #region Minigame 초기화
    /// <summary>
    /// 서버에서 정한 미니게임 선택.
    /// </summary>
    /// <typeparam name="T">IGame의 자식 클래스</typeparam>
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

    //미니게임 맵 설정 - kh
    private void SetMap(string gameType)
    {

    }

    //배경음 설정 - kh
    private void SetBGM(string gameType)
    {

    }
    #endregion

    #region Update
    //현재 점수 변경 - kh
    public void ChangeScore(int _score)
    {

    }

    //현재 시간 변경 - kh
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
