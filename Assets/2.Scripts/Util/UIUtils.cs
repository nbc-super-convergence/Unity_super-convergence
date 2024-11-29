using System.Collections;
using TMPro;
using UnityEngine;

public static class UIUtils
{
    public static IEnumerator DecreaseTimeCoroutine(float time, TextMeshProUGUI timeTxt)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            timeTxt.text = time.ToString();
            yield return null;
        }
    }
}