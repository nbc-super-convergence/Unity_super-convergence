using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientText_Order : MonoBehaviour
{
    List<DiceGameData> playersTmp = new()
    {
        new DiceGameData()
        {
            SessionId = "Play1",
            Distance = 30f
        },
        new DiceGameData()
        {
            SessionId = "Play2",
            Distance = 30f
        }
    };
}
