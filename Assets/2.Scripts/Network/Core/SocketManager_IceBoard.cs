public partial class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //201 : IceMiniGameStartRequest
    //Send 위치 : GameManager or BoardManager

    public void IceMiniGameReadyNotification(GamePacket gamePacket)
    {//202
        var response = gamePacket.IceMiniGameReadyNotification;

        //ReadyUI 띄우기
        //맵 불러오기
        //카메라 세팅?

        foreach (var p in response.Players)
        {//미니 토큰 위치 초기화
            //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(p.PlayerType);
            //miniToken.transform.position = ToVector3(p.Position);
            //miniToken.Controller.RotateY(p.Rotation);
        }
    }

    //203 : IceGameReadyRequest
    //Send 위치 : UIMinigameReady? UIMiniDesc?

    public void IceGameReadyNotification(GamePacket gamePacket)
    {//204
        var response = gamePacket.IceGameReadyNotification;

        //ReadyUI와 연계
        //int readyId = response.sessionId;
    }

    public void IceMiniGameStartNotification(GamePacket gamePacket)
    {//205
        var response = gamePacket.IceMiniGameStartNotification;
        
        //ReadyUI 숨기기
    }

    //206 : IcePlayerSyncRequest
    //Send 위치 : MiniToken

    public void IcePlayerSyncNotification(GamePacket gamePacket)
    {//207
        var response = gamePacket.IcePlayerSyncNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.SessionId);
        //miniToken.transform.position = ToVector3(response.Position);
        //miniToken.Controller.RotateY(response.Rotation);
        //miniToken.MiniData.CurState = response.State;
    }

    //208 : IcePlayerDamageRequest
    //Send 위치 : MapFloor?

    public void IcePlayerDamageNotification(GamePacket gamePacket)
    {//209
        var response = gamePacket.IcePlayerDamageNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.SessionId);
        //해당 플레이어의 HP 1스택 깎기.
    }

    public void IcePlayerDeathNotification(GamePacket gamePacket)
    {//210
        var response = gamePacket.IcePlayerDeathNotification;

        //MiniToken miniToken = MinigameManager.Instance.GetMiniPlayer(response.SessionId);
        //TODO : 사망 로직
    }

    public void IceGameOverNotification(GamePacket gamePacket)
    {//211
        var response = gamePacket.IceGameOverNotification;

        //게임 종료 이벤트 + 판넬
        foreach (var r in response.Ranks)
        {
            //PlayerId받아서 Rank 표시.
        }

        //기존 맵 삭제, MiniToken은 삭제x.
    }

    public void IceMapSyncNotification(GamePacket gamePacket)
    {//212
        //var response = gamePacket.IceMapSyncNotification;
        //맵 사이즈 줄이기.
    }
}
