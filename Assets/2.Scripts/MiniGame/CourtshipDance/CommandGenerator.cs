using System;
using System.Collections.Generic;

public class CommandGenerator
{
    private int[] rotations = { 0, 90, 180, 270 };
    private Random random = new();

    public Queue<Queue<BubbleInfo>> GenerateBoardPool(int poolCount)
    {
        Queue<Queue<BubbleInfo>> pool = new();
        for (int i = 0; i < poolCount; ++i)
        {
            pool.Enqueue(GenerateBoard(i + 5));
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

    public void SetBoardPoolColor(Queue<Queue<BubbleInfo>> pool, int color0, int color1 = -1, int color2 = -1, int color3 = -1)
    {
        foreach ( var queue in pool)
        {
            SetBoardColor(queue, color0, color1, color2, color3);
        }
    }

    private void SetBoardColor(Queue<BubbleInfo> queue, int color0, int color1 = -1, int color2 = -1, int color3 = -1)
    {
        List<int> colors = new List<int> { color0 };
        if(color1 != -1) colors.Add(color1);
        if (color2 != -1) colors.Add(color2);
        if (color3 != -1) colors.Add(color3);

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