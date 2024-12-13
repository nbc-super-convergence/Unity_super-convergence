using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientTest_Order : Singleton<ClientTest>
{
    private void Start()
    {

    }

    private IEnumerator SelectOrderSkipper()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.LeftShift))
            {
                GamePacket packet = new()
                {
                    DiceGameNotification = new()
                    {
                        Result =
                        {
                            new DiceGameData
                            {
                                SessionId = "Session1",
                                Rank = 4,
                                Distance = 0,
                                Angle = new Vector { X = 0, Y = 0, Z = 0 },
                                Location = new Vector { X = 0, Y = 0, Z = 0 },
                                Power = 0,
                            },
                            new DiceGameData
                            {
                                SessionId = "Session2",
                                Rank = 2,
                                Distance = 0,
                                Angle = new Vector { X = 0, Y = 0, Z = 0 },
                                Location = new Vector { X = 0, Y = 0, Z = 0 },
                                Power = 0,
                            },
                            new DiceGameData
                            {
                                SessionId = "Session3",
                                Rank = 1,
                                Distance = 0,
                                Angle = new Vector { X = 0, Y = 0, Z = 0 },
                                Location = new Vector { X = 0, Y = 0, Z = 0 },
                                Power = 0,
                            },
                            new DiceGameData
                            {
                                SessionId = "Session4",
                                Rank = 3,
                                Distance = 0,
                                Angle = new Vector { X = 0, Y = 0, Z = 0 },
                                Location = new Vector { X = 0, Y = 0, Z = 0 },
                                Power = 0,
                            }
                        }
                    }
                };

                SocketManager.Instance.DiceGameNotification(packet);
                break;
            }
            yield return null;
        }

        while(true)
        {

        }
    }
}
