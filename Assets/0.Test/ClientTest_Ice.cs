using static S2C_IceMiniGameReadyNotification.Types;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public partial class ClientTest : Singleton<ClientTest>
{
    private IEnumerator IceBoardSkipper()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BoardScene");

        //IceMinigame으로 넘어가기
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

        //다음 상호작용
    }
}