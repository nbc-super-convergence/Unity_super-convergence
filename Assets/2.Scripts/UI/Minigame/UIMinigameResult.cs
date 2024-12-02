using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMinigameResult : UIBase
{
    [SerializeField] private GameObject[] RankPanels;

    public override void Opened(object[] param)
    {
        //ranksÀÇ string : sessionId, int : µî¼ö
        if (param.Length > 0 && param[0] is Dictionary<string, int> ranks)
        {
            
        }
    }
}