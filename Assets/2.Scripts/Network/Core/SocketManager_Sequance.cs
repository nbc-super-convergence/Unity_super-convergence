using UnityEngine;

public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //플레이어 순서 정하기

    public void DiceGameResponse(GamePacket packet)
    {
        var response = packet.DiceGameResponse;

        if(response.Success)
        {
            var result = response.Result;

            Debug.Log(result);

            int i = 0;
            var playerDart = MinigameManager.Instance.GetMiniGame<GameDart>().DartOrder;

            foreach(var dart in playerDart)
            {
                dart.MyDistance = result[i].Distance;
                dart.MyRank = result[i].Rank;
                dart.CurAim = ToVector3(result[i].Angle);
                dart.transform.localPosition = ToVector3(result[i].Location);
                dart.CurForce = result[i].Power;
                i++;
            }
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void DiceGameNotification(GamePacket packet)
    {
        var response = packet.DiceGameNotification;
        var result = response.Result;

        MinigameManager.Instance.SetMiniGame<GameDart>(response);

        Debug.Log(result);
    }

    public void DiceGameRequest(GamePacket packet)
    {
        var request = packet.DiceGameRequest;
    }
}
