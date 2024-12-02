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
    }

    public void MapChangeEvent()
    {
        //맵 크기 변경 
        gameData.phase--;
        MinigameManager.Instance.GetMap<MapGameIceSlider>()
            .MapDecreaseEvent(gameData.phase);

        //시간 차이 감지
        ingameUI.CheckTime();
    }
    
    public void GameEnd()
    {

    }
}