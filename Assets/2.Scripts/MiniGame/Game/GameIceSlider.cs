using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIceSlider : IGame
{
    private MiniGameData gameData;
    private int[] playerHps = new int[4]; //¿œ¥‹ maxHP = 10

    public void InitGame(MiniGameData data)
    {
        gameData = data;
    }
}