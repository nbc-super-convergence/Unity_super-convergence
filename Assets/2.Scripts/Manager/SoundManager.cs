using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMType
{
    StartScene,
    BoardScene,
    Ice,
    Dropper,
    Dance,
    Bomb,
    Dart,
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    public AudioClip[] bgmClip;

    private AudioClip currentBGM;
    private Tween fadeTween;
    private int sfxCount = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    #region 메인
    public void PlayBGM(BGMType type, float fadeDuration = 1f)
    {
        AudioClip clip = bgmClip[(int)type];
        if (currentBGM == clip) return; // 이미 재생 중인 클립이면 무시

        if (bgmSource.isPlaying)
        {
            // 페이드 아웃 처리
            fadeTween?.Kill();
            fadeTween = bgmSource.DOFade(0f, fadeDuration)
                .OnComplete(() =>
                {
                    bgmSource.Stop();
                    bgmSource.clip = clip;
                    currentBGM = clip;

                    // 새 클립 페이드 인 처리
                    bgmSource.Play();
                    fadeTween = bgmSource.DOFade(1f, fadeDuration);
                });
        }
        else
        {
            // 바로 새 클립 재생
            bgmSource.clip = clip;
            currentBGM = clip;
            bgmSource.volume = 0f;
            bgmSource.Play();
            fadeTween = bgmSource.DOFade(1f, fadeDuration);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxCount >= 10) return; // SFX 재생 제한

        sfxSource.PlayOneShot(clip, volume);
        sfxCount++;

        // DOTween으로 딜레이 후 카운트 감소
        DOTween.To(() => 0f, x => { }, 0f, clip.length)
            .OnComplete(() => sfxCount--)
            .SetAutoKill(true);
    }
    #endregion

    #region 오디오 믹서
    public void SetBGMAudioMixerValue(float value)
    {
        if (value == 0)
            audioMixer.SetFloat("BGM", -80f);
        else
            audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    public void SetSFXAudioMixerValue(float value)
    {
        if (value == 0)
            audioMixer.SetFloat("SFX", -80f);
        else
            audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }
    #endregion
}