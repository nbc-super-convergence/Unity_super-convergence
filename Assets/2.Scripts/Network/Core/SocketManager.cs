using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GamePacket;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    //TODO: 새로운 함수 만들 때 private void로 만들 것.
    //TODO: 함수의 이름은 반드시 PayloadOneOfCase Enum과 맞출 것.
    //TODO: 인자는 GamePacket gamePacket.

    //나의 접속 Send.
    public void IceJoinRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IceJoinRequest;
    }

    //다른 플레이어 스폰 Receive.
    public void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;
    }

    //나의 플레이어 움직임 Send.
    public void IcePlayerMoveRequest(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveRequest;
    }

    //다른 플레이어 움직임 Receive.
    public void IcePlayerMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveNotification;
    }

    //새로운 벡터 선언 (임시로)
    public static Vector CreateVector(Vector3 other)
    {
        return new Vector
        {
            X = other.x,
            Y = other.y,
            Z = other.z,
        };
    }
}