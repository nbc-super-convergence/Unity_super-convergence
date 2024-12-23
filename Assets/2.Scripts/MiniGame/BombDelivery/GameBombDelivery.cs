using Google.Protobuf.Collections;
using UnityEngine;
using static S2C_BombMiniGameReadyNotification.Types;

public class GameBombDelivery : IGame
{
    private Bomb bomb;

    public void GameStart(params object[] param)
    {
        SoundManager.Instance.PlayBGM(BGMType.Bomb);
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
        bomb.gameObject.SetActive(true);
    }

    public async void Init(params object[] param)
    {
        //var tokens = MinigameManager.Instance.miniTokens;

        //for (int i = 0; i < 4; i++)
        //    tokens[i].MiniData.PlayerSpeed = 15;

        ResetSpeed();

        var prefab = await ResourceManager.Instance.LoadAsset<Bomb>("bomb", eAddressableType.Prefab);
        bomb = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        bomb.gameObject.SetActive(false);

        MinigameManager.Instance.curMap = 
            await ResourceManager.Instance.LoadAsset<MapGameBombDelivery>
            ($"Map{MinigameManager.gameType}",eAddressableType.Prefab);

        MinigameManager.Instance.MakeMap<MapGameBombDelivery>();

        if (param.Length > 0 && param[0] is S2C_BombMiniGameReadyNotification response)
        {
            SetPlayer(response.Players);
        }
        else
        {
            Debug.LogError("startPlayers 자료형 전달 과정에서 문제 발생");
        }
    }

    public void SetPlayer(RepeatedField<startPlayers> players)
    {
        for(int i = 0; i < players.Count; i++)
        {
            MiniToken token = MinigameManager.Instance.GetMiniToken(players[i].SessionId);
            token.EnableMiniToken();
            token.transform.localPosition = SocketManager.ToVector3(players[i].Position);
            token.MiniData.nextPos = SocketManager.ToVector3(players[i].Position);
            token.MiniData.rotY = players[i].Rotation;
            token.MiniData.CurState = State.Idle;
        }
        SetTarget(players[0].BombSessionId);
        bomb.gameObject.SetActive(false);
    }

    public void Explosion(string id)
    {
        bomb.gameObject.SetActive(false);
        bomb.Explosion(id);
    }

    public void SetTarget(string id)
    {
        bomb.SetTarget(id);
        bomb.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        //var tokens = MinigameManager.Instance.miniTokens;

        //for(int i = 0; i < 4; i++)
        //    tokens[i].MiniData.PlayerSpeed = 15;

        ResetSpeed();

        Object.Destroy(bomb.gameObject);
        Object.Destroy(MinigameManager.Instance.curMap.gameObject);
    }

    public void DisableUI()
    {
        
    }

    private void ResetSpeed()
    {
        var tokens = MinigameManager.Instance.miniTokens;

        for (int i = 0; i < 4; i++)
            tokens[i].MiniData.PlayerSpeed = 15;
    }
}
