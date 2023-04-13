using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    
    public List<Player> players;                            //플레이어 정보 리스트
    private List<Tile> atkTiles;                            //공격 가능한 타일 리스트

    private Vector3 mousePos;                               //마우스 위치
    public Player selectPlayer {get; set;}                  //선택된 플레이어의 정보

    private bool attackOn = false;                          //공격 버튼 활/비활성화

    Tile[,] tiles;
    private List<Tile> moveTiles;                           //움직일 수 있는 타일 리스트

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        atkTiles = new List<Tile>();
        moveTiles = new List<Tile>();
    }

    private void Start()
    {
        StartCoroutine(GameStart());
    }
    //게임을 시작하면 플레이어의 정보를 리스트에 올린다.
    IEnumerator GameStart()
    {
        GameObject[] goPlayers = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i <goPlayers.Length; ++i)
        {
            players.Add(goPlayers[i].GetComponent<Player>());
        }

        InitSelectPlayer();

        yield return null;

        ShowTile();
    }
    private void Update()
    {
        //내턴이고 OnPlay상태에서만 먹히게
        if (GameMgr.instance.IsPlayerTurn && GameMgr.instance.OnPlay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MoveTile();
                SelectPlayer();
                AttackEnemy();
            }
        }
    }
    //SelectedPlayer 초기화시키기
    public void InitSelectPlayer()
    {
        selectPlayer = players[0];
        GameManager.instance.OnPlay = false;
    }
    //타일 보여주기 (매턴 첫 시작용)
    public void ShowTile()
    {
        //스타트포인트 꺼주고
        TempMap.instance.StartPointsOff();

        Tile[,] tiles = TempMap.instance.tiles;
        GameMgr.instance.OnPlay = true;
        moveTiles = TempMap.instance.ShowMoveableTile(tiles[selectPlayer.tileX, selectPlayer.tileY], selectPlayer.moveCount);
    }
    //타일 이동
    private void MoveTile()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

        //움직임 중이거나 Selected플레이어 없거나, 공격버튼 활성화 상태면
        if (selectPlayer.isMove == true || selectPlayer == null || attackOn)
            return;

        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit == false)
            return;

        if (hit.collider.CompareTag("WalkableTile"))
        {
            Tile tempTile = hit.collider.gameObject.GetComponent<Tile>();

            //타일위에 적이나 아군 없으면
            if (tempTile.onObject != OnObject.PLAYER && tempTile.onObject != OnObject.ENEMY)
            {
                //활동 끝났으면 리턴
                if (selectPlayer.canMove == false)
                    return;

                //가능한 이동타일이 아니면(tempmap의 함수에 의해 결정됨)
                if (moveTiles.Contains(tempTile) == false)
                    return;

                //길찾기
                AStar.instance.SetStartEndIndex(selectPlayer.tileX, selectPlayer.tileY, tempTile.tileX, tempTile.tileY);
                bool result = AStar.instance.FindPath();

                //길찾기 성공했다면
                if (result)
                {
                    //온타일 세팅
                    selectPlayer.OnTileSet(tempTile);
                    //무빙 함수 호출
                    selectPlayer.MoveTile();
                    //전체타일 색깔 날려주기
                    TempMap.instance.ClearTileColor();
                }
            }
        }
    }
    //플레이어 선택 함수
    public void SelectPlayer()
    {
        //ui위일때는 오브젝트들 안건들게 (panel은 raycast target 해제해주자.. 안그러면 모든화면에 다 먹통됨)
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

        //움직임 중이거나 Selected플레이어 없으면 리턴
        if (SelectedPlayer.MoveOn == true || SelectedPlayer == null)
            return;

        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit == false)
            return;

        if (hit.collider.CompareTag("WalkableTile"))
        {
            Tile tmpTile = hit.collider.gameObject.GetComponent<Tile>();
            //마우스 찍은 타일에 플레이어 있으면
            if (tmpTile.onObject == OnObject.PLAYER)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].OnTileX == tmpTile.tileX && players[i].OnTileY == tmpTile.tileY)
                    {
                        //이미 턴이 끝난 플레이어면 리턴시키자
                        if (players[i].TurnOver)
                            return;

                        //SelectedPlayer 세팅
                        SelectedPlayer = players[i];

                        //아직 움직일 수 있으면
                        if (SelectedPlayer.CanMove == true)
                        {
                            canTiles.Clear();
                            //선택된 캐릭 이동가능타일 보여주기
                            canTiles = TempMap.instance.ShowMoveableTile(tmpTile, SelectedPlayer.MoveCount);
                            Debug.Log(SelectedPlayer.Name);
                            break;
                        }
                        //움직임 끝났으면
                        else
                        {
                            TempMap.instance.ClearTileColor();
                        }
                    }
                }
            }
        }
    }
    //공격 버튼을 눌러 범위 내의 적 클릭시 실행
    public void AttackEnemy()
    {
        //어택버튼 눌른 상태가 아니거나, ui누른거면 리턴..
        if (attackOn == false || EventSystem.current.IsPointerOverGameObject() == true)
            return;

        //어택가능 위치한 에너미타일
        atkTiles = TempMap.instance.attTiles;

        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit == false)
            return;

        if (hit.collider.CompareTag("WalkableTile"))
        {
            Tile tmpTile = hit.collider.gameObject.GetComponent<Tile>();
            //공격 가능한 타일이면
            if (atkTiles.Contains(tmpTile))
            {
                //플레이어의 에너미 세팅후 attack함수 실행
                selectPlayer.enemy = tmpTile.OnEnemy;
                selectPlayer.Attack();
                //공격버튼 누른상태인거 초기화
                attackOn = false;
            }
        }
    }
    //모든 플레이어 행동 가능 초기화
    public void ResetMoveable()
    {
        attackOn = false;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].canMove = true;
            players[i].isTurn = false;
        }
    }
    //대기 버튼 클릭
    public void OnClickStay()
    {
        if (GameManager.instance.IsPlayerTurn == false)
            return;

        //대기,공격버튼 없애기
        UiMgr.instance.panelIngame.SetActive(false);

        //카메라 추적 세팅
        Camera.main.gameObject.GetComponent<CameraFollow>().IsDragging = false;

        //해당 캐릭터 턴오버 세팅
        selectPlayer.isTurn = true;
        //material 회색으로
        selectPlayer.transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = Color.gray;

        //플레이어 중 턴이 오지 않은 캐릭 SelctedPlayer로 세팅
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isTurn == true)
                continue;

            selectPlayer = players[i];
            Tile[,] tiles = TempMap.instance.tiles;
            moveTiles = TempMap.instance.ShowMoveableTile(tiles[selectPlayer.tileX, selectPlayer.tileY], selectPlayer.moveCount);
            return;
        }
        //전부 턴이 끝났다면
        //공격버튼 초기화
        foreach (var p in players)
        {
            p.isAttack = false;
            p.transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = Color.white;
        }

        TempMap.instance.ClearTileColor();
        GameManager.instance.TurnChange();
    }
    //공격 버튼 클릭
    public void OnClickAttack()
    {
        //무빙중, 내턴아님, 이미 공격햇음
        if (!GameMgr.instance.IsPlayerTurn || selectPlayer.isAttack || selectPlayer.isMove)
            return;

        //이동 가능 타일보여주는 것과 안겹치도록
        TempMap.instance.ClearTileColor();

        attackOn = !attackOn;

        if (attackOn)
            TempMap.instance.ShowAttackTile(tiles[selectPlayer.tileX, selectPlayer.tileY], selectPlayer);
        else
            TempMap.instance.ClearTileColor();
    }
}
