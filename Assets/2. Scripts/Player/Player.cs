using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//플레이어 구분을 위한 열거형 변수
public enum PlayerClass
{
    Warrior,            //전사
    Archer,             //궁수
    Wizard,             //법사
    Thief               //도적
}

public class Player : MonoBehaviour
{
    private Animator anim;
    private PlayerInfo info;                                //플레이어의 정보가 있는 데이터 클래스

    [HideInInspector] public Image hpBar;                   //플레이어 체력바
    [HideInInspector] public Image playerFace;              //플레이어 초상화

    public AudioClip deathSound;
    private GameObject deathEff;                            //죽을 때의 이펙트

    public Player enemy { get; set; }                         //전투할 상대의 정보

    private List<Tile> path;                                //길찾기를 위한 타일 리스트

    //플레이어가 있는 타일의 인덱스
    public int tileX { get; set; }
    public int tileY { get; set; }

    public bool isMove { get; set; }                          //이동 확인 변수
    private float moveSpeed = 8f;

    //행동을 한 후에 확인할 변수들
    public bool canMove { get; set; }                         //이동을 한 뒤에 움직일 수 없도록
    public bool isTurn { get; set; }                          //자신의 턴인지 확인할 변수
    public bool isAttack { get; set; }                        //공격 여부 확인
    public bool isAttackReady { get; set; }                 //공격 준비가 되었는지

    //현재 애니메이션 체크용 int 변수
    private int hashRunUp;
    private int hashRunDown;
    private int hashRunLeft;
    private int hashRunRight;

    [HideInInspector] public PlayerClass playerClass;       //클래스 상속을 위함.

    //플레이어의 스테이터스
    public int maxHP { get; set; }
    public int curHP { get; set; }
    public int atk { get; set; }
    public int def { get; set; }
    public int range { get; set; }

    public int moveCount { get; set; }                       //움직일 수 있는 타일의 수
    public string playerName { get; set; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        info = GetComponent<PlayerInfo>();

        hpBar = GetComponent<Image>();
        playerFace = GetComponent<SpriteRenderer>().sprite;

        deathEff = Resources.Load("Boom") as GameObject;

        hashRunUp = Animator.StringToHash("Base Layer.runUp");
        hashRunDown = Animator.StringToHash("Base Layer.runDown");
        hashRunLeft = Animator.StringToHash("Base Layer.runLeft");
        hashRunRight = Animator.StringToHash("Base Layer.runRight");

        isMove = false;
    }

    private void Start()
    {
        //움직일 수 있는 상태로 시작
        canMove = true;

        playerClass = info._playerClass;
        maxHP = info.MaxHP;
        curHP = info.curHP;
        atk = info.Atk;
        def = info.def;
        range = info.Range;
        moveCount = info.movableTileCount;
        name = info._name;
    }
    //플레이어 생성 시 위치 세팅을 위한 함수
    public void SetPlayerPosition(Tile tile)
    {
        this.tag = "Player";

        tileX = tile.tileX;
        tileY = tile.tileY;

        tile.onObject = OnObject.PLAYER;
        tile.onObject = this;
        transform.position = tile.transform.position;
    }
    //적 오브젝트 위치 세팅
    public void SetEnemyPosition(Tile tile)
    {
        this.tag = "Player";

        tileX = tile.tileX;
        tileY = tile.tileY;

        tile.onObject = OnObject.ENEMY;
        tile.onObject = this;
        transform.position = tile.transform.position;
    }
    //타일 이동
    public void MoveTile()
    {
        path;
        isMove = true;
        StartCoroutine(Move());
    }
    //이동 구현을 맡은 코루틴 함수
    IEnumerator Move()
    {
        int index = 0;

        Tile currentTile = path[index];
        Vector3 savePos = transform.position;

        while (index < path.Count)
        {
            Vector3 targetPos = currentTile.transform.position;

            if (targetPos.Equals(transform.position))
            {
                if (index < path.Count - 1)
                {
                    savePos = transform.position;
                    currentTile = path[++index];
                }
                //무빙 끝. 빠져나가자
                else
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hashRunUp)
                        anim.SetTrigger("stayUp");
                    else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hashRunDown)
                        anim.SetTrigger("stayDown");
                    else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hashRunLeft)
                        anim.SetTrigger("stayLeft");
                    else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hashRunRight)
                        anim.SetTrigger("stayRight");

                    isMove = false;
                    canMove = false;

                    //이후 코딩은 에너미턴에만 해당
                    if (GameMgr.instance.IsPlayerTurn == true)
                        yield break;

                    //공격 가능상태면 공격하고
                    if (isAttackReady)
                        Attack();
                    //아니면 다음에너미 턴 가자
                    else
                        EnemyMgr.instance.NextEnemyPlay();

                    yield break;
                }
            }

            Vector3 dir = (targetPos - transform.position).normalized;

            if (dir == Vector3.up)
            {
                if ((anim.GetCurrentAnimatorStateInfo(0).fullPathHash != hashRunUp))
                    anim.SetTrigger("runUp");
            }
            else if (dir == Vector3.down)
            {
                if ((anim.GetCurrentAnimatorStateInfo(0).fullPathHash != hashRunDown))
                    anim.SetTrigger("runDown");
            }
            else if (dir == Vector3.left)
            {
                if ((anim.GetCurrentAnimatorStateInfo(0).fullPathHash != hashRunLeft))
                    anim.SetTrigger("runLeft");
            }
            else if (dir == Vector3.right)
            {
                if ((anim.GetCurrentAnimatorStateInfo(0).fullPathHash != hashRunRight))
                    anim.SetTrigger("runRight");
            }
            transform.Translate(dir * Time.deltaTime * moveSpeed);


            if (Mathf.Abs(Vector3.SqrMagnitude(transform.position - savePos)) >= 1f)
            {
                transform.position = targetPos;
            }

            yield return null;
        }
    }
    //공격 함수
    public void Attack()
    {
        StartCoroutine(CorAttack());
    }
    //공격기능 구현을 맡은 코루틴 함수
    IEnumerator CorAttack()
    {
        //공격 시전
        isAttack = true;
        //EnemyMgr용 공격 준비 변수 초기화
        isAttackReady = false;

        //코루틴으로 전투를 진행시키고 종료까지 대기
        yield return StartCoroutine(UiMgr.instance.BattleStart());

        //플레이어 턴이라면
        if (GameMgr.instance.IsPlayerTurn)
        {
            //다음 플레이어 실행
            PlayerMgr.instance.OnClickStay();
        }
        //적 턴이면
        else
        {
            //다음 적 실행
            EnemyMgr.instance.NextEnemyPlay();
        }
    }
    //피격시 실행되는 함수
    public void TakeDamage(int atk)
    {
        //데미지 = 공격력 - (방어력 / 2)
        int damage = atk - def / 2;
        curHP -= damage;

        //체력이 0보다 낮을 경우 0으로 초기화
        if (curHP <= 0)
        {
            curHP = 0;
        }
        //게임 내에 보이는 HP바에 연동
        hpBar.fillAmount = (float)curHP / maxHP;
        
        //체력바도 체력 변수와 동일하게 초기화 진행
        if (hpBar.fillAmount <= 0f)
        {
            hpBar.fillAmount = 0f;
        }
    }
    //기존 타일 초기화시키고 목적지 타일 세팅하는 함수
    public void OnTileSet(Tile tile)
    {
        //기존 밟고있던 타일 초기화 시켜주기 
        Tile[,] tiles = TempMap.instance.tiles;
        tiles[tileX, tileY].onObject = OnObject.NONE;

        //플레이어와 적 두 가지의 태그가 있으므로 Switch문 이용
        switch (this.gameObject.tag)
        {
            case "Player":
                tiles[tileX, tileY].OnPlayer = null;

                //도착점 타일 세팅
                tile.onObject = OnObject.PLAYER;
                tile.OnPlayer = this;
                break;
            case "Enemy":
                tiles[tileX, tileY].OnEnemy = null;

                //도착점 타일 세팅
                tile.onObject = OnObject.ENEMY;
                tile.OnEnemy = this;
                break;
        }
        //타일 위치 세팅
        tileX = tile.tileX;
        tileY = tile.tileY;
    }
    //사망 여부를 확인하는 함수
    public void DieCheck()
    {
        if (curHP <= 0)
        {
            Debug.Log("으악! 죽음");
            Die();
        }
    }

    void Die()
    {
        SoungMgr.instance.PlayEff(deathSound);

        //터지는 이펙트
        Instantiate(deathEff, transform.position, Quaternion.identity);

        //기존 밟고있던 타일 초기화 및, 매니저 리스트로부터 삭제
        Tile[,] tiles = TempMap.instance.tiles;
        tiles[tileX, tileY].onObject = OnObject.NONE;

        switch (this.gameObject.tag)
        {
            case "Player":
                tiles[tileX, tileY].OnPlayer = null;
                PlayerMgr.instance.players.Remove(this);
                break;
            case "Enemy":
                tiles[tileX, tileY].OnEnemy = null;
                EnemyMgr.instance.enemies.Remove(this);
                break;
        }
        Destroy(this.gameObject);
    }
}