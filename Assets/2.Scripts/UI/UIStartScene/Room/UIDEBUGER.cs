using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIDEBUGER : MonoBehaviour
{
    public Button[] button;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        ShowUI();                
    }

    private async void ShowUI()
    {
        await UIManager.Show<UIRoom>();
    }

    public void ButtonTest()
    {
        TestCode();
    }
    public void ButtonTest2()
    {
        TestCode2();
    }

    private async void TestCode()
    {
        await UIManager.Show<UIError>();
    }

    private async void TestCode2()
    {
        await UIManager.Show<UIError>("이건 어떻게 나올까");
    }
}