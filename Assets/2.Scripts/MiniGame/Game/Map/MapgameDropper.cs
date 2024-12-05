using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameDropper : MapBase
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private List<GameObject> levels;

    [SerializeField] private float cameraFollowDelay = 0.2f;

    public void NextLevelEvent(int phase, int[] holes)
    {
        
    }
}