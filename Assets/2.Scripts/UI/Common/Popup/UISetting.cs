using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIBase
{
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;

    private void OnEnable()
    {
        BGMSlider.value = PlayerPrefs.GetFloat(SoundManager.Instance.BGM_PREFS_KEY, 1f);
        SFXSlider.value = PlayerPrefs.GetFloat(SoundManager.Instance.SFX_PREFS_KEY, 1f);

        BGMSlider.onValueChanged.AddListener(ONBGMChange);
        SFXSlider.onValueChanged.AddListener(ONSFXChange);
    }

    private void OnDisable()
    {
        BGMSlider.onValueChanged.RemoveListener(ONBGMChange);
        SFXSlider.onValueChanged.RemoveListener(ONSFXChange);
    }

    public void OnBackBtn()
    {
        UIManager.Hide<UISetting>();
    }

    private void ONBGMChange(float value)
    {
        SoundManager.Instance.SetBGMAudioMixerValue(value);
        PlayerPrefs.SetFloat(SoundManager.Instance.BGM_PREFS_KEY, value);
        PlayerPrefs.Save();
    }

    private void ONSFXChange(float value)
    {
        SoundManager.Instance.SetSFXAudioMixerValue(value);
        PlayerPrefs.SetFloat(SoundManager.Instance.SFX_PREFS_KEY, value); 
        PlayerPrefs.Save();
    }
}
