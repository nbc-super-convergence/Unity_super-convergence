using System.Collections.Generic;
using UnityEngine;

public class GameIceSlider : IGame
{
    private GameIceSliderData gameData;
    private UIMinigameIce ingameUI;

    public async void Init()
    {
        gameData = new GameIceSliderData();
        gameData.Init();
        MinigameManager.Instance.CurMap = await ResourceManager.Instance.LoadAsset<MapGameIceSlider>($"Map{MinigameManager.GameType}", eAddressableType.Prefab);
        SetBGM();
    }

    //TODO : 배경음 설정
    private void SetBGM()
    {

    }

    public async void GameStart()
    {
        ingameUI = await UIManager.Show<UIMinigameIce>(gameData);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }

    //TODO : Damage 주는 바닥에도 필요.(나의 데미지)
    public void GiveDamage(string sessionId, int dmg, bool isMe = false)
    {
        int idx = GameManager.Instance.SessionDic[sessionId];
        gameData.playerHps[idx] -= dmg;
        ingameUI.ChangeHPUI();

        /*서버 없을 때 임시 로직*/
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
        //맵 크기 변경 
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