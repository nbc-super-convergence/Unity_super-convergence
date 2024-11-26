using TMPro;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public TextMeshProUGUI idText;
    public TextMeshProUGUI force;

    private void Update()
    {
        int id = GameManager.Instance.PlayerId;
        idText.text = id.ToString();
        force.text = MiniGameManager.Instance.GetMiniPlayer(id).curForce.ToString();
    }
}
