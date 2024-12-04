using System.Collections;
using System.Collections.Generic;
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

            //여기서 필요한 데이터
            //순위, 현재 플레이어, 인원수, 다트 거리
            //
        }
        else
        {
            Debug.LogError($"FailCode : {response.FailCode.ToString()}");
        }
    }

    public void DiceGameNotification(GamePacket packet)
    {
        var response = packet.DiceGameNotification;
    }
}
