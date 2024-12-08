using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;

public class CommandGenerator
{
    public int boardAmount = 15;
    public int minBubbleCount = 3;
    public int maxBubbleCount = 13;

    private int[] rotations = { 0, 90, 180, 270 };
    private Random random = new();

    private Dictionary<string, UserInfo> SessionDic = GameManager.Instance.SessionDic;

    private Dictionary<string, Queue<Queue<BubbleInfo>>> playerPoolDic = new();

    public void InitFFA(List<Player> players)
    {
        var originPool = GenerateBoardInfoPool(boardAmount);

        for ( int i = 0; i < players.Count; i++ )
        {
            Queue<Queue<BubbleInfo>> playerPool = DeepCopyPool(originPool);   // 깊은 복사
            SetBoardPoolColor(playerPool, players[i]);
            playerPoolDic.Add(players[i].SessionId, playerPool);
        }
    }

    private Queue<Queue<BubbleInfo>> DeepCopyPool(Queue<Queue<BubbleInfo>> original)
    {
        return new Queue<Queue<BubbleInfo>>(
              original.Select(innerQueue => new Queue<BubbleInfo>(
                      innerQueue.Select(bubble => bubble.Clone())
                   ))
              );
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

    public void SetBoardPoolColor(Queue<Queue<BubbleInfo>> pool, Player player)
    {
        foreach (var queue in pool)
        {
            SetBoardColor(queue, player);
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
        int color = -1;
        
        if (SessionDic.TryGetValue(player.SessionId, out UserInfo userInfo))
        {
            color = userInfo.Color;
        }
        else
        {
            // 게임매니저 세션딕에 찾는 세션Id가 없음.
        }
        
        foreach (BubbleInfo b in queue)
        {
            b.SetColor(color);
            b.SetSessionId(player.SessionId);
        }
    }


    private float GetRandomRotation()
    {
        return rotations[random.Next(rotations.Length)];
    }    
}