using System.Collections.Generic;
using UnityEngine;

public class UICommandBoardHandler : UIBase
{
    private List<CommandBoard> CommandBoards = new();

    public List<Transform> spawnPosition;

    public async void Make(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var board = Instantiate(await ResourceManager.Instance.LoadAsset<CommandBoard>("CommandBoard", eAddressableType.Prefab), spawnPosition[i]);
            board.transform.localPosition = Vector3.zero;
            CommandBoards.Add(board);
            CommandBoards[i].Init();
        }
    }

    public void Next()
    {
        CommandBoards[0].MakeNextBoard();
    }
}