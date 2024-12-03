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
    }

    private IEnumerator BoardSkipper()
    {

        while (true)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.B))
            {
                //������ SessionDic ����
                InitSessionDic();

                //��Ÿ �ʱ�ȭ (�ʿ��ϴٸ�)

                //BoardScene ����
                GameManager.isGameStart = true;
            }
            yield return null;
        }
    }

    
}
#endif