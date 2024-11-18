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

    //다른 플레이어 움직임
    private void IceMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMoveNotification;
        
    }

    //다른 플레이어 스폰
    private void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;
    }

    //게임 시작 (나중에 구현)
    private void IceStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceStartNotification;
    }

    //애니메이션 반영
    private void IcePlayersStateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayersStateSyncNotification;
    }

    //플레이어 사망 이벤트
    private void IcePlayerDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerDeathNotification;
    }

    //맵 상태 이벤트(변경 없으면 유효성, 있으면 변경)
    private void IceMapStateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceMapStateSyncNotification;
    }

    //Ice 미니게임 종료
    private void IceOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IceOverNotification;
    }
}