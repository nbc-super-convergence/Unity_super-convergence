using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

// SFX 정보를 관리하는 구조체


public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource[] bgm;
    [SerializeField] AudioSource sfx;
    public AudioClip[] bgmClip;

    private int activeChannel = 0; // 현재 재생 중인 채널
    private Tween[] fadeInTweens = new Tween[2];  // 각 채널의 페이드 인 Tween
    private Tween[] fadeOutTweens = new Tween[2]; // 각 채널의 페이드 아웃 Tween
    private int sfxCount = 0;

    protected override void Awake()
    {
        base.Awake();
        InitTweens();
    }

    [ContextMenu("OnBGM")]
    public void Test1()
    {
        PlayBGM(bgmClip[0]);
    }

    #region 메인
    public void PlayBGM(AudioClip clip)
    {
        int nextChannel = 1 - activeChannel; // 다음 채널 (0 -> 1, 1 -> 0)

        // 새 클립 설정
        bgm[nextChannel].clip = clip;

        // 페이드 인/아웃 Tween 실행
        fadeInTweens[nextChannel].Restart();
        fadeOutTweens[activeChannel].Restart();

        // 활성 채널 갱신
        activeChannel = nextChannel;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        //추가 설정이 필요할수도...?
        if (sfxCount > 10) return;

        sfx.PlayOneShot(clip, volume);
        sfxCount++;

        // DOTween으로 딜레이 후 카운트 감소
        DOTween.To(() => 0f, x => { }, 0f, clip.length)
            .OnComplete(() => sfxCount--)
            .SetAutoKill(true); // 작업 완료 후 자동 제거
    }
    #endregion

    private void InitTweens()
    {
        for (int i = 0; i < 2; i++)
        {
            // 페이드 인 Tween
            fadeInTweens[i] = bgm[i].DOFade(1f, 1f)
                .SetAutoKill(false)
                .Pause()
                .OnPlay(() => bgm[i].Play());

            // 페이드 아웃 Tween
            fadeOutTweens[i] = bgm[i].DOFade(0f, 1f)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => bgm[i].Stop());
        }
    }

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
