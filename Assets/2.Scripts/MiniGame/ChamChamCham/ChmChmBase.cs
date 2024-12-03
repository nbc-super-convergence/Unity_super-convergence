using UnityEngine;



public class ChmChmBase : MonoBehaviour
{
    [SerializeField] private GameObject ownerBlinder;
    [SerializeField] private GameObject[] otherBlinder;

    public enum YourRole { Owner, Other };
    public YourRole MyRole;
}
