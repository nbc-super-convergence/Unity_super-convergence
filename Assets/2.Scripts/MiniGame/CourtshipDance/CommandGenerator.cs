using System;
using System.Collections.Generic;

public class CommandGenerator
{
    public int minBubbleCount = 3;
    public int maxBubbleCount = 13;

    private int[] rotations = { 0, 90, 180, 270 };
    private Random random = new();

    private Dictionary<string, UserInfo> SessionDic = GameManager.Instance.SessionDic;

    private Dictionary<string, Queue<Queue<BubbleInfo>>> playerPoolDic = new();

    public void InitFFA(List<Player> players)
    {
        var originPool = GenerateBoardInfoPool(players.Count);

        for ( int i = 0; i < players.Count; i++ )
        {
            Queue<Queue<BubbleInfo>> playerPool = new(originPool);
            SetBoardPoolColor(playerPool, players[i]);
            playerPoolDic.Add(players[i].SessionId, playerPool);
        }
    }
    public Dictionary<string, Queue<Queue<BubbleInfo>>> GetPlayerPoolDic()
    {
        return new(playerPoolDic);
    }

    public Queue<Queue<BubbleInfo>> GenerateBoardInfoPool(int poolCount)
    {
        Queue<Queue<BubbleInfo>> pool = new();
        for (int i = 0; i < poolCount; ++i)
        {
            int bubbleCount = Math.Clamp(i, minBubbleCount, maxBubbleCount);
            pool.Enqueue(GenerateBoardInfo(bubbleCount));
        }
        return pool;
    }

    private Queue<BubbleInfo> GenerateBoardInfo(int bubbleCount)
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

    public void SetBoardPoolColor(Queue<Queue<BubbleInfo>> pool, Player players)
    {
        foreach (var queue in pool)
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

    private void SetBoardColor(Queue<BubbleInfo> queue, Player player)
    {
        List<int> colors = new();
        
        if (SessionDic.TryGetValue(player.SessionId, out UserInfo userInfo))
        {
            colors.Add(userInfo.Color);
        }
        
        if (colors.Count == 0) return;

        int colorIndex = 0;

        foreach (BubbleInfo b in queue)
        {
            b.SetColor(colors[colorIndex]);
            b.SetSessionId(player.SessionId);
            colorIndex = (colorIndex + 1) % colors.Count;
        }
    }


    private float GetRandomRotation()
    {
        return rotations[random.Next(rotations.Length)];
    }    
}