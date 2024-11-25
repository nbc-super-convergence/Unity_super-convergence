using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameData
{
    eGameType gameType;
    public string mapKey;
    public string soundKey;
    public string totalTime;


    public abstract void GameStart();
}
