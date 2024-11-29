public class GameIceSlider : IGame
{
    private GameIceSliderData gameData;
    private UIMinigameIce ingameUI;

    public void Init()
    {
        gameData = new GameIceSliderData();
        gameData.Init();
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
        //TODO : Map조작에 관한 인터페이스 + 클래스 필요
        //맵 크기 변경 
        gameData.phase--;
        MinigameManager.Instance.CurMap.transform.localScale = 
            new UnityEngine.Vector3(gameData.phase, 1, gameData.phase);

        //시간 차이 감지
        ingameUI.CheckTime();
    }
    
    public void GameEnd()
    {

    }

}