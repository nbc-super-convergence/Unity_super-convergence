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

    //TODO : ����� ����
    private void SetBGM()
    {

    }

    public async void GameStart()
    {
        ingameUI = await UIManager.Show<UIMinigameIce>(gameData);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }

    //TODO : Damage �ִ� �ٴڿ��� �ʿ�.(���� ������)
    public void GiveDamage(string sessionId, int dmg, bool isMe = false)
    {
        int idx = GameManager.Instance.SessionDic[sessionId];
        gameData.playerHps[idx] -= dmg;
        ingameUI.ChangeHPUI();
    }

    public void MapChangeEvent()
    {
        //�� ũ�� ���� 
        gameData.phase--;
        MinigameManager.Instance.GetMap<MapGameIceSlider>()
            .MapDecreaseEvent(gameData.phase);

        //�ð� ���� ����
        ingameUI.CheckTime();
    }
    
    public void GameEnd()
    {

    }
}