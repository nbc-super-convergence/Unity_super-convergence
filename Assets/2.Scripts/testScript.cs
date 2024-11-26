using TMPro;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public TextMeshProUGUI Text1;
    public TextMeshProUGUI Text2;


    private void Update()
    {
        int id = GameManager.Instance.PlayerId;
        Text1.text = MiniGameManager.Instance.GetMiniPlayer(1).GetForce.ToString();
        Text2.text = MiniGameManager.Instance.GetMiniPlayer(2).GetForce.ToString();
    }
}
