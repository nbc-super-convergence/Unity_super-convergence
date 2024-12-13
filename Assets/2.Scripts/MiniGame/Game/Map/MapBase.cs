using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapBase : MonoBehaviour
{
    public virtual void HandleCollision(eCollisionType type, Collision collision) { }

    public virtual void HandleCollider(eCollisionType type, Collider other) { }
}