using System.Collections.Generic;

public class GameBombDelivery : IGame
{
    public void GameStart(params object[] param)
    {
        MinigameManager.Instance.GetMyToken().EnableInputSystem();
    }

    public async void Init(params object[] param)
    {
        MinigameManager.Instance.curMap = 
            await ResourceManager.Instance.LoadAsset<MapGameBombDelivery>
            ($"Map{MinigameManager.gameType}",eAddressableType.Prefab);

        MinigameManager.Instance.MakeMap();
    }
}
