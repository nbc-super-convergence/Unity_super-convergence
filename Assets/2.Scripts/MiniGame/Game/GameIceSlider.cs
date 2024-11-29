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
        //TODO : Map���ۿ� ���� �������̽� + Ŭ���� �ʿ�
        //�� ũ�� ���� 
        gameData.phase--;
        MinigameManager.Instance.CurMap.transform.localScale = 
            new UnityEngine.Vector3(gameData.phase, 1, gameData.phase);

        //�ð� ���� ����
        ingameUI.CheckTime();
    }
    
    public void GameEnd()
    {

    }

}