using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDartPanel : MonoBehaviour
{
    private List<DiceGameData> sendServerData;  //서버에 전송할 데이터

    //다트판 속성
    private float xPositionLimit = 0.5f;  //옆으로 이동하기까지 제한
    private float pannelSpeed = 0.3f;   //다트판 이동 속도
    private bool swapDirection = false;
    public bool isMove = true;  //움직이고 있는지
    private Vector3 moveDirection = Vector3.zero;
    private bool imClient;  //내 차례면 이 클라이언트에서 움직이게

    private void Awake()
    {
        sendServerData = new List<DiceGameData>();
    }

    private void OnCollisionEnter(Collision collision)
    {   //다음 차례
        MinigameManager.Instance.GetMiniGame<GameDart>().NextDart();
    }

    public void MoveEvent(Vector pos)
    {
        transform.localPosition = SocketManager.ToVector3(pos);
    }

    /// <summary>
    /// 실시간으로 판넬 움직이기
    /// </summary>
    public IEnumerator MoveCoroutine()
    {
        while(isMove)
        {
            if(imClient)
            {
                if (transform.localPosition.x < -xPositionLimit)
                    moveDirection = Vector3.right;
                else if (transform.localPosition.x > xPositionLimit)
                    moveDirection = Vector3.left;

                ApplyMove();
                if (imClient) SendServer();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                //StopCoroutine(MoveCoroutine());
            }
        }
    }

    public void SetClient(string sessionId)
    {
        imClient = MinigameManager.Instance.mySessonId.Equals(sessionId);
    }

    private void ApplyMove()
    {
        transform.Translate(moveDirection * (Time.deltaTime * pannelSpeed));
    }

    /// <summary>
    /// 서버로 전송
    /// </summary>
    private void SendServer()
    {
        GamePacket packet = new();
        {
            packet.DartPannelSyncRequest = new()
            {
                SessionId = MinigameManager.Instance.mySessonId,
                Location = SocketManager.ToVector(transform.localPosition)
            };
        }
        SocketManager.Instance.OnSend(packet);
    }
}