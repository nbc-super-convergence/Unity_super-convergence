using UnityEngine;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    /* 소켓 매니저 가이드
     * 새로운 함수 만들 때 public void로 만들 것. (public 사유 : 리플렉션의 정상 작동)
     * 함수명 PayloadOneOfCase Enum과 맞출 것.
     * 인자는 GamePacket gamePacket.
     */

    #region Receive Packets
    //나의 스폰 알림 Receive.
    public void IcePlayerSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerSpawnNotification;

        //추후 로그인 단계로 이동.
        GameManager.Instance.SetPlayerId(response.PlayerId); 

        //init Minigame
        MiniGameManager.Instance.SetMiniGame<GameIceSlider>();

        //Spawn MiniPlayer
        MiniGameManager.Instance.GetMiniPlayer(response.PlayerId)
            .ReceivePlayerSpawn(ConvertVector3(response.Position), response.Rotation);
    }

    //다른 플레이어 움직임 Receive.
    public void IcePlayerMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.IcePlayerMoveNotification;
        foreach (var p in response.Players)
        {
            Debug.LogWarning($"ReceiveClientMove : {p.PlayerId}");
            MiniGameManager.Instance.GetMiniPlayer(p.PlayerId)
                .ReceiveOtherMove(ConvertVector3(p.Position), ConvertVector3(p.Force), p.Rotation, p.State);
        }
    }
    #endregion

    #region Parse Messages
    //새로운 벡터 선언 (임시로)
    public static Vector ConvertVector(Vector3 other)
    {
        return new Vector
        {
            X = other.x,
            Y = other.y,
            Z = other.z,
        };
    }

    public static Vector3 ConvertVector3(Vector other)
    {
        return new Vector3()
        {
            x = other.X,
            y = other.Y,
            z = other.Z
        };
    }
    #endregion
}