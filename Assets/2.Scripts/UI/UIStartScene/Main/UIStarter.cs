using System.Collections;
using UnityEngine;

public class UIStarter : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        ShowUIStart();
        Destroy(gameObject);
    }

    private async void ShowUIStart()
    {
        await UIManager.Show<UIStart>();
    }
}


