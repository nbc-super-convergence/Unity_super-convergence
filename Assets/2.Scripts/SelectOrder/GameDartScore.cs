using System;
using System.Collections.Generic;

public class GameDartScore
{
    public List<List<float>> scoreInfo = new List<List<float>>();

    public GameDartScore(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            scoreInfo.Add(new List<float>());
        }
    }

    // 점수기록
    public void RecordingScore(string sessionId, int color, float distance)
    {
        scoreInfo[color].Add(distance);
        // TODO:: 점수표UI를 갱신시키는 코드
        UIManager.Get<UIMinigameDart>().AddScore(color, scoreInfo[color].Count, distance);

        float allScore = 0f;
        foreach(float score in scoreInfo[color])
        {
            allScore += score;
        }

        #region 패킷 보내는 코드
        GamePacket packet = new()
        {
            DartPointRequest = new()
            {
                SessionId = sessionId,
                Point = CalculateScore(allScore)   //이걸 어떻게 계산할까?
            }
        };
        SocketManager.Instance.OnSend(packet);
        #endregion
    }

    private int CalculateScore(float score)
    {
        //Todo :: 받은 거리에서 정수형으로 점수 환산하기
        
        return Convert.ToInt16(1 / score);
    }
}