using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static S2C_IceMiniGameReadyNotification.Types;

#if UNITY_EDITOR
public class ClientTest : Singleton<ClientTest>
{
    private void Start()
    {
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
                GameManager.isGameStart = true;
            }
        }
    }

    private IEnumerator IceBoardSkipper()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BoardScene");

        //IceMinigame���� �Ѿ��
        while (true)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.I))
            {
                GamePacket packet = new()
                {
                    IceMiniGameReadyNotification = new()
                    {
                        Players =
                        {
                            new startPlayers
                            {
                                SessionId = "Session1",
                                Position = new Vector { X = 1.0f, Y = 0.0f, Z = 1.0f },
                                Rotation = 45.0f
                            },
                            new startPlayers
                            {
                                SessionId = "Session2",
                                Position = new Vector { X = 1.0f, Y = 0.0f, Z = 1.0f },
                                Rotation = 45.0f
                            },
                            new startPlayers
                            {
                                SessionId = "Session3",
                                Position = new Vector { X = 1.0f, Y = 0.0f, Z = 1.0f },
                                Rotation = 45.0f
                            },
                            new startPlayers
                            {
                                SessionId = "Session4",
                                Position = new Vector { X = 1.0f, Y = 0.0f, Z = 1.0f },
                                Rotation = 45.0f
                            }
                        }
                    }
                };

                SocketManager.Instance.IceMiniGameReadyNotification(packet);
                break;
            }
            yield return null; 
        }

        //���� ��ȣ�ۿ�
    }
}
#endif