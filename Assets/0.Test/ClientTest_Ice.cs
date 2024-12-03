using static S2C_IceMiniGameReadyNotification.Types;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public partial class ClientTest : Singleton<ClientTest>
{
    private IEnumerator IceBoardSkipper()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BoardScene");

        //IceMiniGameReadyNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
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
                                Position = new Vector { X = -4, Y = 0.2f, Z = 4 },
                                Rotation = 135
                            },
                            new startPlayers
                            {
                                SessionId = "Session2",
                                Position = new Vector { X = 4, Y = 0.2f, Z = 4 },
                                Rotation = -135
                            },
                            new startPlayers
                            {
                                SessionId = "Session3",
                                Position = new Vector { X = 4, Y = 0.2f, Z = -4 },
                                Rotation = -45
                            },
                            new startPlayers
                            {
                                SessionId = "Session4",
                                Position = new Vector { X = -4, Y = 0.2f, Z = -4 },
                                Rotation = 45
                            }
                        }
                    }
                };

                SocketManager.Instance.IceMiniGameReadyNotification(packet);
                break;
            }
            yield return null;
        }

        //IceMiniGameStartNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.O))
            {
                GamePacket packet = new GamePacket();
                SocketManager.Instance.IceMiniGameStartNotification(packet);
                break;
            }
            yield return null;
        }

        //IceMapSyncNotification
        //IcePlayerExitNotification

        //IceGameOverNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.P))
            {
                GamePacket packet = new()
                {
                    IceGameOverNotification = new()
                    {
                        Ranks =
                        {
                             
                        }
                    }
                };
                SocketManager.Instance.IceGameOverNotification(packet);
                break;
            }
            yield return null;
        }
    }
}