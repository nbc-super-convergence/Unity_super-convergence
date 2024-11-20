using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

// SFX ������ �����ϴ� ����ü


public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource[] bgm;
    [SerializeField] AudioSource sfx;
    public AudioClip[] bgmClip;

    private int activeChannel = 0; // ���� ��� ���� ä��
    private Tween[] fadeInTweens = new Tween[2];  // �� ä���� ���̵� �� Tween
    private Tween[] fadeOutTweens = new Tween[2]; // �� ä���� ���̵� �ƿ� Tween
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

    #region ����
    public void PlayBGM(AudioClip clip)
    {
        int nextChannel = 1 - activeChannel; // ���� ä�� (0 -> 1, 1 -> 0)

        // �� Ŭ�� ����
        bgm[nextChannel].clip = clip;

        // ���̵� ��/�ƿ� Tween ����
        fadeInTweens[nextChannel].Restart();
        fadeOutTweens[activeChannel].Restart();

        // Ȱ�� ä�� ����
        activeChannel = nextChannel;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        //�߰� ������ �ʿ��Ҽ���...?
        if (sfxCount > 10) return;

        sfx.PlayOneShot(clip, volume);
        sfxCount++;

        // DOTween���� ������ �� ī��Ʈ ����
        DOTween.To(() => 0f, x => { }, 0f, clip.length)
            .OnComplete(() => sfxCount--)
            .SetAutoKill(true); // �۾� �Ϸ� �� �ڵ� ����
    }
    #endregion

    private void InitTweens()
    {
        for (int i = 0; i < 2; i++)
        {
            // ���̵� �� Tween
            fadeInTweens[i] = bgm[i].DOFade(1f, 1f)
                .SetAutoKill(false)
                .Pause()
                .OnPlay(() => bgm[i].Play());

            // ���̵� �ƿ� Tween
            fadeOutTweens[i] = bgm[i].DOFade(0f, 1f)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => bgm[i].Stop());
        }
    }

    #region ����� �ͼ�
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
