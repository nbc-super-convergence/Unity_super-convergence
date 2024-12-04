using System;
using System.Collections.Generic;

public class CommandGenerator
{
    private int[] rotations = { 0, 90, 180, 270 };
    private Random random = new();

    private Dictionary<string, UserInfo> SessionDic = GameManager.Instance.SessionDic;

    public Queue<Queue<BubbleInfo>> GenerateBoardPool(int poolCount)
    {
        Queue<Queue<BubbleInfo>> pool = new();
        for (int i = 0; i < poolCount; ++i)
        {
            int bubbleCount = Math.Clamp(i, 0, 13);
            pool.Enqueue(GenerateBoard(bubbleCount));
        }
        return pool;
    }

    private Queue<BubbleInfo> GenerateBoard(int bubbleCount)
    {
        Queue<BubbleInfo> queue = new();
        for (int i = 0; i < bubbleCount; ++i)
        {
            queue.Enqueue(new BubbleInfo(GetRandomRotation()));
        }
        return queue;
    }

    public void SetBoardPoolColor(Queue<Queue<BubbleInfo>> pool, List<Player> players)
    {
        foreach ( var queue in pool)
        {
            SetBoardColor(queue, players);
        }
    }
        
    private void SetBoardColor(Queue<BubbleInfo> queue, List<Player> teamPlayers)
    {
        List<int> colors = new();
        foreach ( var player in teamPlayers)
        {
            if(SessionDic.TryGetValue(player.SessionId, out UserInfo userInfo))
            {
                colors.Add(userInfo.Color);
            }
        }
        if (colors.Count == 0) return;

        int colorIndex = 0;

        foreach (BubbleInfo b in queue)
        {
            b.SetColor(colors[colorIndex]);
            colorIndex = (colorIndex + 1) % colors.Count;
        }
    }

    private float GetRandomRotation()
    {
        return rotations[random.Next(rotations.Length)];
    }    
}