using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIBase
{
    [SerializeField] private Image loadingImage;
    [SerializeField]
    private bool isStarted = false;

    [SerializeField]
    private Slider loadingSlider;
    [SerializeField]
    private WaitForSeconds loadingTerm = new WaitForSeconds(0.3f);

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        PlayLoadingScreen();
    }

    private async void PlayLoadingScreen()
    {
        if (!isStarted)
        {
            isStarted = true;
            StartCoroutine(FakeSlider());
            loadingImage.gameObject.SetActive(false);
            loadingSlider.gameObject.SetActive(false);
            await UIManager.Show<UIStart>();
            Destroy(this);
        }
        else
        {
            //로딩화면 완료시 로딩화면 종료
            StartCoroutine(FakeSlider());
        }
    }

    private IEnumerator FakeSlider()
    {
        loadingSlider.value = 0;
        while (loadingSlider.value < 1)
        {
            loadingSlider.value += 0.2f;
            yield return loadingTerm;
        }

        yield return 0;
    }

    

    public override void Opened(object[] param)
    {
        base.Opened(param);
        PlayLoadingScreen();
    }
}
