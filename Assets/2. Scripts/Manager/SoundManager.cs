using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioEffect;
    public AudioSource audioBGM;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    //이펙트 사운드 재생 함수
    public void PlayEffectSound(AudioClip clip)
    {
        audioEffect.clip = clip;
        audioEffect.Play();
    }
    //배경 사운드 재생 함수
    public void PlayBGMSound(AudioClip clip)
    {
        audioBGM.clip = clip;
        audioBGM.loop = true;
        audioBGM.Play();
    }
}
