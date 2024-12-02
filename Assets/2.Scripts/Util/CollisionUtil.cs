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
                map.HandleCollision(collision, type);
            }
        }
    }
}