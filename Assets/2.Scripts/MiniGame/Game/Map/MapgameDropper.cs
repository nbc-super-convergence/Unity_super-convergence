using System.Collections;
using UnityEngine;

public class MapGameDropper : MapBase
{
    [SerializeField] private Transform iceBoard;

    [SerializeField] private float phaseTime = 1f;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float inputDelay = 1f;

    public void MapDecreaseEvent(int phase)
    {
        
    }

}