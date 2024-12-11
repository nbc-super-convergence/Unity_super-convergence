using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public partial class ClientTest : Singleton<ClientTest>
{
    private IEnumerator IceBoardSkipper()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BoardScene");

        //DropMiniGameReadyNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.I))
            {
                GamePacket packet = new()
                {
                    DropMiniGameReadyNotification = new()
                    {
                        Players =
                        {
                            new S2C_DropMiniGameReadyNotification.Types.startPlayers
                            {
                                SessionId = "Session1",
                                Slot = 0,
                                Rotation = 0
                            },
                            new S2C_DropMiniGameReadyNotification.Types.startPlayers
                            {
                                SessionId = "Session2",
                                Slot = 2,
                                Rotation = 0
                            },
                            new S2C_DropMiniGameReadyNotification.Types.startPlayers
                            {
                                SessionId = "Session3",
                                Slot = 6,
                                Rotation = 0
                            },
                            new S2C_DropMiniGameReadyNotification.Types.startPlayers
                            {
                                SessionId = "Session4",
                                Slot = 8,
                                Rotation = 0
                            }
                        }
                    }
                };

                SocketManager.Instance.DropMiniGameReadyNotification(packet);
                break;
            }
            yield return null;
        }

        //DropMiniGameStartNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.O))
            {
                GamePacket packet = new ()
                {
                    DropMiniGameStartNotification = new()
                    {
                        StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5000
                    }
                };
                SocketManager.Instance.DropMiniGameStartNotification(packet);
                break;
            }
            yield return null;
        }

        //DropLevelEndNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.I))
            {
                GamePacket packet = new()
                {
                    DropLevelEndNotification = new()
                    {
                        Holes = {0, 1, 3, 5, 7}
                    }
                };
                SocketManager.Instance.DropLevelEndNotification(packet);
                break;
            }
            yield return null;
        }

        //DropLevelEndNotification
        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) &&
            Input.GetKeyDown(KeyCode.O))
            {
                GamePacket packet = new()
                {
                    DropPlayerSyncNotification = new()
                    {
                        SessionId = "Session1",
                        Slot = 1,
                        Rotation = 0,
                        State = 0,
                    }
                };
                SocketManager.Instance.DropPlayerSyncNotification(packet);
                break;
            }
            yield return null;
        }
    }
}