using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
public partial class ClientTest : Singleton<ClientTest>
{
    private void Start()
    {
        StartCoroutine(BoardSkipper());
        StartCoroutine(IceBoardSkipper());
    }

    private void InitSessionDic()
    {
        GameManager.Instance.AddNewPlayer("Session1", "Player1", 0, 0);
        GameManager.Instance.AddNewPlayer("Session2", "Player2", 1, 1);
        GameManager.Instance.AddNewPlayer("Session3", "Player3", 2, 2);
        GameManager.Instance.AddNewPlayer("Session4", "Player4", 3, 3);

        //GameManager.Instance.myInfo = GameManager.Instance.SessionDic["Session1"];
    }

    private IEnumerator BoardSkipper()
    {

        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.B))
            {
                //임의의 SessionDic 설정
                InitSessionDic();

                //기타 초기화 (필요하다면)

                //BoardScene 진입
                GameManager.isGameStart = true;
            }
            yield return null;
        }
    }

    
}
#endif