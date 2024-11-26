using TMPro;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public TextMeshProUGUI id;

    private void Update()
    {
        id.text = GameManager.Instance.PlayerId.ToString();
    }
}
