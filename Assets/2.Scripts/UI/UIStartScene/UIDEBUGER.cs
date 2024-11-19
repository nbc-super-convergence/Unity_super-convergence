using UnityEngine;
using UnityEngine.UI;

public class UIDEBUGER : MonoBehaviour
{
    public Button[] button;

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
