using System;
using UnityEngine;

public enum eGameType
{//���� Ŭ������� ��ġ�ؾ� ��. ���� ���� �ٲ��� �� ��.
    GameIceSlider,
    GameBombDelivery,
}

public class MinigameManager : Singleton<MinigameManager>
{
    [SerializeField] private Transform MiniParent; //�̴ϰ��� ������Ʈ �θ�
    public MapBase CurMap;

    /*�̴ϰ��� ����*/
    public static eGameType GameType { get; private set; } //���� ����
    private IGame curMiniGame; //�̴ϰ��� ���� �޼��� ȣ���
    
    [SerializeField] public MiniToken[] MiniTokens { get; private set; } //�̴ϰ��� ĳ����
    public string MySessonId
    {
        get { return MySessonId; }
        set
        {
            if (MySessonId == null)
            {
                MySessonId = value;
            }
            else
            {
                Debug.LogWarning("�̹� mySessonId ������ �� ����");
            }
        }
    }

    #region Properties
    public T GetMiniGame<T>() where T : IGame
    {
        return (T)curMiniGame;
    }

    public MiniToken GetMiniToken(string sessionId)
    {
        if (GameManager.Instance.SessionDic.TryGetValue(sessionId, out int idx))
            return MiniTokens[idx];
        else
            return null;
    }

    public MiniToken GetMyToken()
    {
        int idx = GameManager.Instance.SessionDic[MySessonId];
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
    public T SetMiniGame<T>() where T : IGame, new()
    {
        GameType = (eGameType)Enum.Parse(typeof(eGameType), nameof(T));
        curMiniGame = new T();
        curMiniGame.Init();
        MakeMap();

        return (T)curMiniGame;
    }

    //�̴ϰ��� �� ����
    private void MakeMap()
    {
        Instantiate(CurMap.gameObject, MiniParent);
    }
    #endregion
}