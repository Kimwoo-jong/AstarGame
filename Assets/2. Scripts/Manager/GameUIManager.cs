using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [Header("In GameScene Panel")]
    public GameObject battlePnl;
    public GameObject inGamePnl;
    public GameObject selectPnl;
    public GameObject turnPnl;
    public GameObject winPnl;
    public GameObject losePnl;

    //패널의 UI 정보
    [Header("Player UI Information")]
    public Image playerFace;
    public Image playerHpBar;
    public Text playerAtkTxt;
    public Text playerDefTxt;
    public Text playerHpTxt;

    [Header("Enemy UI Information")]
    public Image enemyFace;
    public Image enemyHpBar;
    public Text enemyAtkTxt;
    public Text enemyDefTxt;
    public Text enemyHpTxt;

    //턴 별 프로필의 위치 확인
    [Header("Player Profile Check")]
    private Image atkFace;              //공격하는 캐릭터 얼글 표시
    private Image atkHpBar;             //공격 캐릭터 HP바
    private Text atkHpTxt;              //공격 캐릭터 HP 텍스트
    private Image defFace;              //방어하는 캐릭터 얼굴 표시
    private Image defHpBar;             //공격 캐릭터 HP바
    private Text defHpTxt;              //공격 캐릭터 HP 텍스트

    private Player attacker;
    private Player defender;

    [Header("Game Sound")]
    public AudioClip battleBGM;         //전투 BGM
    public AudioClip normalBGM;         //일반 BGM
    public AudioClip audioSlashEff;     //베는 효과음

    public GameObject slashEffect;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SoundManager.instance.PlayBGMSound(normalBGM);
    }
    //전투 시작
    public IEnumerator StartBattle()
    {
        //배경음 변경
        SoundManager.instance.PlayBGMSound(battleBGM);
        SetBeforeBattle();

        yield return new WaitForSeconds(1f);

        //공격자의 사거리가 방어자보다 길 경우
        if(attacker.range > defender.range)
        {
            //실제 거리를 계산
            int length = Mathf.Abs(attacker.tileX - defender.tileX) + Mathf.Abs(attacker.tileY - defender.tileY);

            //플레이어의 사거리만큼 떨어져 있다면
            if(attacker.range == length)
            {
                defender.TakeDamage(attacker.atk);
                UIInstantiate(slashEffect, defFace.transform);
            }
            //상대의 사거리 내에 내가 들어와 있다면
            else
            {
                attacker.TakeDamage(defender.atk);
                defender.TakeDamage(attacker.atk);

                UIInstantiate(slashEffect, atkFace.transform);
                UIInstantiate(slashEffect, defFace.transform);
            }
        }
        //사거리가 같을 경우
        else if(attacker.range == defender.range)
        {
            attacker.TakeDamage(defender.atk);
            defender.TakeDamage(attacker.atk);

            UIInstantiate(slashEffect, atkFace.transform);
            UIInstantiate(slashEffect, defFace.transform);
        }
        //공격자의 사거리가 방어자보다 짧을 경우
        else
        {
            if(attacker.range == 1)
            {
                attacker.TakeDamage(defender.atk);
                //근접 공격시 불리할 수 있으므로.
                defender.TakeDamage(attacker.atk * 2);
            }
            else
            {
                attacker.TakeDamage(defender.atk);
                defender.TakeDamage(attacker.atk);
            }
            UIInstantiate(slashEffect, atkFace.transform);
            UIInstantiate(slashEffect, defFace.transform);
        }

        SoundManager.instance.PlayEffectSound(audioSlashEff);

        yield return new WaitForSeconds(4f);

        SoundManager.instance.PlayBGMSound(normalBGM);
        //전투가 끝나면 패널들을 꺼준다.
        BattlePanelOff();

        attacker.DieCheck();
        defender.DieCheck();

        //승리의 경우
        //적의 수가 0
        if(EnemyManager.instance.enemies.Count == 0)
        {
            GameManager.instance.isOver = true;
            StartCoroutine(GameWin(true));
        }
        //패배의 경우
        //플레이어의 수가 0
        else if(PlayerManager.instance.players.Count == 0)
        {
            GameManager.instance.isOver = true;
            StartCoroutine(GameWin(false));
        }
    }
    //게임의 승리 패배 확인
    public IEnumerator GameWin(bool isWin)
    {
        yield return new WaitForSeconds(1.5f);

        if (isWin)
        {
            winPnl.SetActive(true);
        }
        else
        {
            losePnl.SetActive(true);
        }

        Time.timeScale = 0;
    }
    public void BattlePanelOff()
    {
        battlePnl.SetActive(false);
        inGamePnl.SetActive(true);
    }
    //배틀 시작 직전에 세팅을 위한 함수
    public void SetBeforeBattle()
    {
        battlePnl.SetActive(true);
        inGamePnl.SetActive(false);
        
        //플레이어가 공격자인 경우
        if(GameManager.instance.isPlayerTurn)
        {
            attacker = PlayerManager.instance.selectPlayer;
            defender = PlayerManager.instance.selectPlayer.enemy;
            
            //플레이어 UI 세팅
            playerFace.sprite = attacker.playerFace;          
            playerAtkTxt.text = attacker.atk.ToString();
            playerDefTxt.text = attacker.def.ToString();
            playerHpTxt.text = attacker.curHP.ToString();

            atkFace = playerFace;
            atkHpBar = playerHpBar;
            atkHpTxt = playerHpTxt;
            
            //적 UI 세팅
            enemyFace.sprite = defender.playerFace;
            enemyAtkTxt.text = defender.atk.ToString();
            enemyDefTxt.text = defender.def.ToString();
            enemyHpTxt.text = defender.curHP.ToString();

            defFace = enemyFace;
            defHpBar = enemyHpBar;
            defHpTxt = enemyHpTxt;
        }
        //상대의 턴
        else
        {
            attacker = EnemyManager.instance.selectEnemy;
            defender = EnemyManager.instance.selectEnemy.enemy;

            playerFace.sprite = defender.playerFace;
            playerAtkTxt.text = defender.atk.ToString();
            playerDefTxt.text = defender.def.ToString();
            playerHpTxt.text = defender.curHP.ToString();

            atkFace = enemyFace;
            atkHpBar = enemyHpBar;
            atkHpTxt = enemyHpTxt;

            //적 UI 세팅
            enemyFace.sprite = attacker.playerFace;
            enemyAtkTxt.text = attacker.atk.ToString();
            enemyDefTxt.text = attacker.def.ToString();
            enemyHpTxt.text = attacker.curHP.ToString();

            defFace = playerFace;
            defHpBar = playerHpBar;
            defHpTxt = playerHpTxt;
        }
        
        UpdatePanelState();
    }
    //UI에서의 오브젝트 생성을 위한 함수
    private void UIInstantiate(GameObject obj, Transform parent)
    {
        GameObject o = Instantiate(obj);
        o.transform.SetParent(parent, false);
    }
    public void UpdatePanelState()
    {
        atkHpBar.fillAmount = (float)attacker.curHP / attacker.maxHP;
        if(atkHpBar.fillAmount <= 0f)
        {
            atkHpBar.fillAmount = 0f;
        }

        defHpBar.fillAmount = (float)defender.curHP / defender.maxHP;
        if(defHpBar.fillAmount <= 0f)
        {
            defHpBar.fillAmount = 0f;
        }

        atkHpTxt.text = attacker.curHP.ToString();
        defHpTxt.text = defender.curHP.ToString();
    }
}
