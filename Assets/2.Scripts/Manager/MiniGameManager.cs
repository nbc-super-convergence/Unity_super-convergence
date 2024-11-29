using System;
using UnityEngine;

public enum eGameType
{//���� Ŭ������� ��ġ�ؾ� ��. ���� ���� �ٲ��� �� ��.
    GameIceSlider,
    GameBombDelivery,
}

public class MinigameManager : Singleton<MinigameManager>
{
    [SerializeField] private Transform MapParent; //�� ������Ʈ instantiate��ġ
    public GameObject CurMap { get; private set; }

    /*�̴ϰ��� ����*/
    public eGameType GameType { get; private set; } //���� ����
    private IGame curMiniGame; //�̴ϰ��� ���� �޼��� ȣ���
    
    [SerializeField] private MiniToken[] miniTokens; //�̴ϰ��� ĳ����
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
            return miniTokens[idx];
        else
            return null;
    }

    public MiniToken GetMyToken()
    {
        int idx = GameManager.Instance.SessionDic[MySessonId];
        return miniTokens[idx];
    }
    #endregion

    #region Minigame �ʱ�ȭ
    /// <summary>
    /// �������� ���� �̴ϰ��� ����.
    /// </summary>
    /// <typeparam name="T">IGame�� �ڽ� Ŭ����</typeparam>
    public T SetMiniGame<T>() where T : IGame, new()
    {
        GameType = (eGameType)Enum.Parse(typeof(eGameType), nameof(T));
        curMiniGame = new T();
        curMiniGame.Init();
        SetMap(nameof(T));

        return (T)curMiniGame;
    }

    //�̴ϰ��� �� ����
    private async void SetMap(string gameType)
    {
        GameObject map = await ResourceManager.Instance.LoadAsset<GameObject>($"Map{gameType}", eAddressableType.Prefab);
        CurMap = Instantiate(map, MapParent);
    }
    #endregion
}
