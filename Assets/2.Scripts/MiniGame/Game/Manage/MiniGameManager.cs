using System;
using UnityEngine;

public enum eGameType
{//���� Ŭ������� ��ġ�ؾ� ��. ���� ���� �ٲ��� �� ��.
    Default,
    GameIceSlider,
    GameBombDelivery
}

public class MinigameManager : Singleton<MinigameManager>
{
    [SerializeField] private Transform MiniParent; //�̴ϰ��� ������Ʈ �θ�
    public GameObject boardCamera; //������� ī�޶�
    public MapBase CurMap;

    /*�̴ϰ��� ����*/
    public static eGameType GameType { get; private set; } //���� ����
    private IGame curMiniGame; //�̴ϰ��� ���� �޼��� ȣ���

    public MiniToken[] MiniTokens; //�̴ϰ��� ĳ����
    public string MySessonId => GameManager.Instance.myInfo.SessionId;

    #region Properties
    public T GetMiniGame<T>() where T : IGame
    {
        return (T)curMiniGame;
    }

    public MiniToken GetMiniToken(string sessionId)
    {
        if (GameManager.Instance.SessionDic.TryGetValue(sessionId, out UserInfo idx))
            return MiniTokens[idx.Color];
        else
            return null;
    }

    public MiniToken GetMyToken()
    {
        int idx = GameManager.Instance.SessionDic[MySessonId].Color;
        return MiniTokens[idx];
    }

    public T GetMap<T>() where T : MapBase
    {
        return (T)CurMap;
    }
    #endregion

    #region Minigame �ʱ�ȭD
    /// <summary>
    /// �������� ���� �̴ϰ��� ����.
    /// </summary>
    /// <typeparam name="T">IGame�� �ڽ� Ŭ����</typeparam>
    public T SetMiniGame<T>(params object[] param) where T : IGame, new()
    {
        GameType = (eGameType)Enum.Parse(typeof(eGameType), typeof(T).Name);
        curMiniGame = new T();
        curMiniGame.Init(param);
        
        return (T)curMiniGame;
    }

    //�̴ϰ��� �� ����
    public void MakeMap()
    {
        Instantiate(CurMap.gameObject, MiniParent);
    }
    #endregion
}