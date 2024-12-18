using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMType
{
    Login,
    Lobby,
    Room,
    BoardScene,
    Ice,
    Dropper,
    Dance,
    Bomb,
    Dart,
    End,
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

    private const string volumeMaster = "volumeMaster";
    private const string volumeBGM = "volumeBGM";
    private const string volumeSFX = "volumeSFX";

    public readonly string BGM_PREFS_KEY = "BGMVolume";
    public readonly string SFX_PREFS_KEY = "SFXVolume";

    protected override void Awake()
    {
        base.Awake();

        
    }

    private void Start()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_PREFS_KEY, 1f);
        SetBGMAudioMixerValue(bgmVolume);

        float sfxVolume = PlayerPrefs.GetFloat(SFX_PREFS_KEY, 1f);
        SetSFXAudioMixerValue(sfxVolume);
    }

    #region 메인
    public void PlayBGM(BGMType type, float fadeDuration = 1f)
    {
        AudioClip clip = bgmClip[(int)type];
        if (currentBGM == clip) return; // 이미 재생 중인 클립이면 무시

        fadeTween?.Kill(); // 기존 페이드 애니메이션 종료

        if (bgmSource.isPlaying)
        {
            // 기존 BGM 페이드 아웃과 동시에 새로운 클립 준비
            fadeTween = bgmSource.DOFade(0f, fadeDuration / 2)
                .OnComplete(() =>
                {
                    bgmSource.clip = clip;
                    currentBGM = clip;

                    // 페이드 인 처리
                    bgmSource.Play();
                    bgmSource.DOFade(1f, fadeDuration / 2);
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
            audioMixer.SetFloat(volumeBGM, -80f);
        else
            audioMixer.SetFloat(volumeBGM, Mathf.Log10(value) * 20);
    }

    public void SetSFXAudioMixerValue(float value)
    {
        if (value == 0)
            audioMixer.SetFloat(volumeSFX, -80f);
        else
            audioMixer.SetFloat(volumeSFX, Mathf.Log10(value) * 20);
    }
    #endregion
}