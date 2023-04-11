using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpenManager : MonoBehaviour
{
    public AudioClip[] sound;                           //오디오 클립 배열(배경음, 버튼 클릭음)
    private Text text;

    private void Awake()
    {
        text = GameObject.Find("Text").GetComponent<Text>();
    }

    private void Start()
    {
        SoundManager.instance.PlayBGMSound(sound[0]);
    }
    //시작 버튼 눌렀을 때
    public void OnClickPlay()
    {
        SceneManager.LoadScene("scGame");
        SoundManager.instance.PlayEffectSound(sound[1]);
    }
}
