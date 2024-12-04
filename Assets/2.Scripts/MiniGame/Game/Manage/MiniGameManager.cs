using System;
using UnityEngine;

public enum eGameType
{//���� Ŭ������� ��ġ�ؾ� ��. ���� ���� �ٲ��� �� ��.
    Default,
    GameIceSlider,
    GameBombDelivery,
    GameCourtshipDance
}

public class MinigameManager : Singleton<MinigameManager>
{
    #region Field
    public GameObject boardCamera; //������� ī�޶�
    [SerializeField] private Transform miniParent; //hiearchy �θ�
    
    public static eGameType gameType { get; private set; } //���� ���� enum
    public IGame curMiniGame; //Ư�� �̴ϰ��� ���� ��ɱ���
    public MapBase curMap; //�̴ϰ��� �� ������Ʈ ���� ��ɱ���

    public MiniToken[] miniTokens; //�̴ϰ��� ĳ����
    public string mySessonId => GameManager.Instance.myInfo.SessionId;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnPlayerLeft += PlayerLeftEvent;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerLeft -= PlayerLeftEvent;
    }

    #region Properties
    /// <summary>
    /// curMiniGame T�� ��ȯ
    /// </summary>
    public T GetMiniGame<T>() where T : IGame
    {
        return (T)curMiniGame;
    }

    /// <summary>
    /// sessionId�� �´� �̴���ū
    /// </summary>
    public MiniToken GetMiniToken(string sessionId)
    {
        if (GameManager.Instance.SessionDic.TryGetValue(sessionId, out UserInfo info))
            return miniTokens[info.Color];
        else
            return null;
    }

    /// <summary>
    /// Ŭ�� �̴���ū
    /// </summary>
    public MiniToken GetMyToken()
    {
        int idx = GameManager.Instance.SessionDic[mySessonId].Color;
        return miniTokens[idx];
    }

    /// <summary>
    /// curMap T�� ��ȯ
    /// </summary>
    public T GetMap<T>() where T : MapBase
    {
        return (T)curMap;
    }
    #endregion

    #region Minigame �ʱ�ȭ
    /// <summary>
    /// �������� ���� �̴ϰ��� ���� �� �ʱ�ȭ
    /// </summary>
    /// <typeparam name="T">IGame�� �ڽ� Ŭ����</typeparam>
    public T SetMiniGame<T>(params object[] param) where T : IGame, new()
    {
        gameType = (eGameType)Enum.Parse(typeof(eGameType), typeof(T).Name);
        curMiniGame = new T();
        curMiniGame.Init(param);
        
        return (T)curMiniGame;
    }

    //�̴ϰ��� �� ����
    public void MakeMap()
    {
         GameObject instantiatedMap = Instantiate(curMap.gameObject, miniParent);
        curMap = instantiatedMap.GetComponent<MapGameIceSlider>();
    }
    #endregion

    private void PlayerLeftEvent(int color)
    {
        miniTokens[color].gameObject.SetActive(false);
    }
}