using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;                   //싱글톤 패턴

    [Header("Panel")]
    public GameObject battlePanel;                      //배틀 패널
    public GameObject gamePanel;                        //게임 패널
    public GameObject playSelectPanel;                  //플레이어 선택 패널
    public GameObject turnPanel;                        //턴 알림 패널
    public GameObject winPanel;                         //승리 패널
    public GameObject losePanel;                        //패배 패널

    [Header("Player UI Datas")]
    public Image playerImg;
    public Image playerHpBar;
    public Text playerAtk;
    public Text playerDef;
    public Text playerHp;

    [Header("Enemy UI Datas")]
    public Image enemyImg;
    public Image enemyHpBar;
    public Text enemyAtk;
    public Text enemyDef;
    public Text enemyHp;

    [Header("Battle Profile")]
    private Image atkFace;
    private Image atkHpBar;
    private Text atkHp;
    private Image defFace;
    private Image defHpBar;
    private Text defHp;

    [Header("Panel Player")]
    private Player attack;                              //공격자
    private Player defense;                             //수비자

    [Header("Sound")]
    public AudioClip normalBGM;                         //기본 BGM
    public AudioClip battleBGM;                         //전투 BGM
    public AudioClip audioEffect;                       //효과음

    [Header("Effect Prefab")]
    public GameObject slashEffect;                      //베기 이펙트

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //첫 시작 시 기본 BGM 재생
        SoundManager.instance.PlayBGMSound(normalBGM);
    }
    //패널의 상태 업데이트를 위한 함수
    //플레이어나 적 오브젝트의 UI를 변경한다.
    public void UpdatePanelState()
    {

    }
}
