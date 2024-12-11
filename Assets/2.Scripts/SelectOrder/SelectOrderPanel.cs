using System.Collections.Generic;
using UnityEngine;

public class SelectOrderPanel : MonoBehaviour
{
    private List<DiceGameData> sendServerData;  //서버에 전송할 데이터

    //다트판 속성
    private float xPositionLimit = 0.5f;  //옆으로 이동하기까지 제한
    private float pannelSpeed = 0.3f;   //다트판 이동 속도
    private bool swapDirection = false;
    public bool isMove = true;  //움직이고 있는지

    private void Awake()
    {
        sendServerData = new List<DiceGameData>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //다음 차례
        MinigameManager.Instance.GetMiniGame<GameDart>().NextDart();
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            //다트판 이동하기 (세로 한쪽만 던지면 시시하니까)
            //좌우로 왔다갔다 하게
            if (transform.position.x < -xPositionLimit)
                swapDirection = true;
            else if (transform.position.x > xPositionLimit)
                swapDirection = false;

            transform.Translate((swapDirection ? Vector3.right : Vector3.left) * (Time.deltaTime * pannelSpeed));
        }
    }

    private void SendServer()
    {
        GamePacket packet = new();
    }
}