using TMPro;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public TextMeshProUGUI Text1;
    public TextMeshProUGUI Text2;

    private void Update()
    {
        Text1.text = GameManager.Instance.PlayerId.ToString();
    }
}
