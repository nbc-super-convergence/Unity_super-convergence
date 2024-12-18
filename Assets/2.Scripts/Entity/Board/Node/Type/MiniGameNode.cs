using UnityEngine;

public class MiniGameNode : BaseNode
{
    public override void Action()
    {
        int c = BoardManager.Instance.Curplayer.queue.Count;
        int d = BoardManager.Instance.Curplayer.dice;

        if (c > 1 || d > 0)
        {
            base.Action();
            return;
        }

        BoardManager.Instance.StartMinigame();
        //StartCoroutine(BoardManager.Instance.StartMinigame());
        //BoardManager.Instance.TurnEnd();

        //GamePacket packet = new();

        //packet.StartMiniGameRequest = new()
        //{
        //    SessionId = GameManager.Instance.myInfo.SessionId,
        //};

        //SocketManager.Instance.OnSend(packet);
    }
}
