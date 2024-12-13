using TMPro;
using UnityEngine;

public class UserPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;

    public void SetName(string name)
    {
        nameTxt.text = name;
    }
}
