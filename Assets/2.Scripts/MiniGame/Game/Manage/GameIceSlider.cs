using Google.Protobuf.Collections;
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

        if (param.Length > 0 && param[0] is S2C_IceMiniGameReadyNotification response)
        {
            ResetPlayers(response.Players);
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

    public void ResetPlayers(RepeatedField<startPlayers> players)
    {
        foreach (var p in players)
        {//�̴� ��ū ��ġ �ʱ�ȭ
            MiniToken miniToken = MinigameManager.Instance.GetMiniToken(p.SessionId);
            miniToken.EnableMiniToken();
            miniToken.transform.localPosition = SocketManager.ToVector3(p.Position);
            miniToken.MiniData.nextPos = SocketManager.ToVector3(p.Position);
            miniToken.MiniData.rotY = p.Rotation;
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
        gameData.phase++; //�� ���� �ܰ�
        MinigameManager.Instance.GetMap<MapGameIceSlider>()
            .MapDecreaseEvent(gameData.phase);
    }
}