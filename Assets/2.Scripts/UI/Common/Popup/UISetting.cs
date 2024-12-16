using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIBase
{
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;

    private void OnEnable()
    {
        BGMSlider.onValueChanged.AddListener(ONBGMChange);
        BGMSlider.onValueChanged.AddListener(ONSFXChange);
    }

    public void OnBackBtn()
    {
        UIManager.Hide<UISetting>();
    }

    private void ONBGMChange(float value)
    {
        SoundManager.Instance.SetBGMAudioMixerValue(value);
    }
    private void ONSFXChange(float value)
    {
        SoundManager.Instance.SetSFXAudioMixerValue(value);
    }
}
