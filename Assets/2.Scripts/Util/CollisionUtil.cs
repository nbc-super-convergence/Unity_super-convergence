using System.Collections;
using UnityEngine;

public enum eCollisionType
{
    Bounce,
    Damage
}

public class CollisionUtil : MonoBehaviour
{
    [SerializeField] MapBase map;
    [SerializeField] eCollisionType type;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MiniToken mini))
        {
            if (mini.IsClient)
            {
                map.HandleCollision(type, collision);
            }
        }
    }

    public Coroutine OneSecondCoroutine;
    private void OnTriggerStay(Collider other)
    {
        OneSecondCoroutine = StartCoroutine(OneSecondInvoker(other));
    }

    private IEnumerator OneSecondInvoker(Collider other)
    {
        if (other.gameObject.TryGetComponent(out MiniToken mini))
        {
            if (mini.IsClient)
            {
                map.HandleCollider(type);
                yield return new WaitForSeconds(1);
            }
        }
        OneSecondCoroutine = null;
    }
}