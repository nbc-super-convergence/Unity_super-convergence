using UnityEngine;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    /* 소켓 매니저 가이드
     * 새로운 함수 만들 때 public void로 만들 것. (public 사유 : 리플렉션의 정상 작동)
     * 함수명 PayloadOneOfCase Enum과 맞출 것.
     * 인자는 GamePacket gamePacket.
     */

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