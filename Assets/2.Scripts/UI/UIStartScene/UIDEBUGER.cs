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
        //Destroy(gameObject);

        if(FindObjectOfType<UIStarter>())
        {
            GameObject starter = FindObjectOfType<UIStarter>().gameObject;
            starter.SetActive(false);
        }
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
        UIManager.Get<UIError>().SetErrorMessage(1);
    }
    private async void TestCode2()
    {
        await UIManager.Show<UIError>("이건 어떻게 나올까");
    }

}
