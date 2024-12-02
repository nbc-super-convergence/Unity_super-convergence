using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapBase : MonoBehaviour
{
    public abstract void HandleCollision(Collision collision, eCollisionType type);
}