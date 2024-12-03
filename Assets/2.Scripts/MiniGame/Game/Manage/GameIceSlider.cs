using System.Collections.Generic;
using UnityEngine;
using static S2C_IceMiniGameReadyNotification.Types;

public class GameIceSlider : IGame
{
    private GameIceSliderData gameData;
    private UIMinigameIce ingameUI;

    public async void Init(params object[] param)
    {
        gameData = new GameIceSliderData();
        gameData.Init();
        MinigameManager.Instance.CurMap = await ResourceManager.Instance.LoadAsset<MapGameIceSlider>($"Map{MinigameManager.GameType}", eAddressableType.Prefab);
        MinigameManager.Instance.MakeMap();
        SetBGM();

        if (param.Length > 0 && param[0] is startPlayers[] players)
        {
            ResetPlayers(players);
        }
        else
        {
            Debug.LogError("startPlayers �ڷ��� ���� �������� ���� �߻�");
        }
    }

    //TODO : ����� ����
    private void SetBGM()
    {

    }

    public void ResetPlayers(startPlayers[] players)
    {
        foreach (var p in players)
        {//�̴� ��ū ��ġ �ʱ�ȭ
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            miniToken.Controller.SetPos(SocketManager.ToVector3(p.Position));
            miniToken.Controller.SetRotY(p.Rotation);
        }
    }

    public async void GameStart()
    {
        ingameUI = await UIManager.Show<UIMinigameIce>(gameData);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }

    //TODO : Damage �ִ� �ٴڿ��� �ʿ�.(���� ������)
    public void GiveDamage(string sessionId, int dmg, bool isMe = false)
    {
        int idx = GameManager.Instance.SessionDic[sessionId].Color;
        gameData.playerHps[idx] -= dmg;
        ingameUI.ChangeHPUI();

        /*���� ���� �� �ӽ� ����*/
        if (gameData.playerHps[idx] == 0)
        {
            MinigameManager.Instance.GetMiniGame<GameIceSlider>()
            .PlayerDeath(sessionId);
        }
    }

    public void PlayerDeath(string sessionId)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(sessionId);
        if (sessionId == MinigameManager.Instance.MySessonId)
        {
            token.DisableMyToken();
        }
        token.DisableMiniToken();
    }

    public void MapChangeEvent()
    {
        //�� ũ�� ���� 
        gameData.phase--;
        MinigameManager.Instance.GetMap<MapGameIceSlider>()
            .MapDecreaseEvent(gameData.phase);
    }
    
    public async void GameEnd(Dictionary<string, int> ranks)
    {
        foreach (var mini in MinigameManager.Instance.MiniTokens)
        {
            mini.gameObject.SetActive(false);
        }

        await UIManager.Show<UIMinigameResult>(ranks);
        
    }
}