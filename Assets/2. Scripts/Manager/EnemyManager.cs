using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Player> enemies;

    private List<Player> players;                       //PlayerManager의 플레이어
    private Player target;                              //타겟이 될 플레이어

    private Dictionary<int, int> distDic;               //거리 체크용 딕셔너리

    private List<Tile> moveTiles;                       //이동 가능한 타일

    public Player selectEnemy { get; set; }             //플레이 중인 적

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        moveTiles = new List<Tile>();
        enemies = new List<Player>();
        players = new List<Player>();
        distDic = new Dictionary<int, int>();
    }

    private void Start()
    {
        GameObject[] tempEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < tempEnemies.Length; ++i)
        {
            enemies.Add(tempEnemies[i].GetComponent<Player>());
        }

        Tile[,] tiles = TempMap.instance.tiles;

        //적의 스폰 지점 임의 설정
        enemies[0].SetEnemyPosition(tiles[4, 13]);
        enemies[1].SetEnemyPosition(tiles[15, 12]);
        enemies[2].SetEnemyPosition(tiles[19, 10]);

        selectEnemy = enemies[0];
    }
    //선택한 적 초기화
    public void InitSelectEnemy()
    {
        for (int i = 0; i < enemies.Count; ++i)
        {
            enemies[i].isTurnOver = false;
        }

        selectEnemy = enemies[0];
    }
    //플레이어 추적
    public void FindPlayer()
    {
        selectEnemy.isAttackReady = false;
        distDic.Clear();

        players = PlayerManager.instance.players;

        //플레이어가 없다면 멈춤
        if (players.Count == 0)
        {
            return;
        }

        //가장 가까운 플레이어를 타겟으로 한다.
        for (int i = 0; i < players.Count; ++i)
        {
            int dist = Mathf.Abs(selectEnemy.tileX - players[i].tileX) + Mathf.Abs(selectEnemy.tileY - players[i].tileY);
            distDic.Add(i, dist);
        }

        //플레이어의 거리를 토대로 가까운 타깃을 세팅할 것이다.
        //LINQ 기능을 활용하여 제작해 보려고 한다.
        //from 어떤 데이터에서 원하는 값을 추출할 것인지
        //orderby 정렬 연산자로 ascending : 오름차순, descending : 내림차순
        //select 데이터의 어떤 항목을 추출할 것인지

        //여기서는 거리를 오름차순으로 정렬한다.
        var sorting = from pair in distDic
                      orderby pair.Value ascending
                      select pair;

        //정렬한 데이터의 첫번째 키 값을 가져와 타겟에 넣어준다.
        //가장 가까운 거리의 오브젝트가 될 것임.
        target = players[sorting.First().Key];

        StartCoroutine(MoveTile());
    }
    private IEnumerator MoveTile()
    {
        yield return new WaitForSeconds(1.5f);

        Tile[,] tiles = TempMap.instance.tiles;

        Astar.instance.SetStartnTargetIndex(selectEnemy.tileX, target.tileX, selectEnemy.tileY, target.tileY);
        bool result = Astar.instance.PathFinding();

        //경로를 받아와서, 이동 거리 이상의 타일은 없애준다.
        List<Tile> path = Astar.instance.BestWayList;

        selectEnemy.enemy = path.Last().OnPlayer;

        //사거리와 타겟까지의 거리가 동일할 때
        if (path.Count == selectEnemy.range)
        {
            selectEnemy.Attack();
            yield break;
        }
        //사거리가 거리보다 클 경우
        else if (path.Count < selectEnemy.range)
        {
            //적 기준으로 사거리만큼 떨어진 이동 가능한 타일을 구한다.
            moveTiles = TempMap.instance.GetRangeTile(path.Last(), selectEnemy.range);

            int x = selectEnemy.tileX;
            int y = selectEnemy.tileY;

            for (int i = 0; i < moveTiles.Count; ++i)
            {
                Astar.instance.SetStartnTargetIndex(x, moveTiles[i].tileX, y, moveTiles[i].tileY);
                result = Astar.instance.PathFinding();

                if (result)
                {
                    path = Astar.instance.BestWayList;

                    //최적의 경로를 갈 수 있는 이동 거리라면 목적지가 된다.
                    if (selectEnemy.moveCount >= path.Count)
                    {
                        //사거리만큼 간 곳으로 세팅
                        selectEnemy.OnTileSet(path.Last());

                        selectEnemy.isAttackReady = true;
                        selectEnemy.MoveTile();

                        yield break;
                    }
                }
            }
            //조건에 해당하는 타일이 없을 경우 공격 시작
            selectEnemy.Attack();
            yield break;
        }

        //찾은 거리가 이동 거리보다 클 경우
        if (path.Count > selectEnemy.moveCount)
        {
            //타겟과의 남은 거리
            int dist = path.Count - selectEnemy.moveCount;

            //사거리와 남은 거리가 일치할 경우
            if (dist == selectEnemy.range)
            {
                path.RemoveRange(selectEnemy.moveCount, dist);
                //이동 후에 공격이 가능하도록 한다.
                selectEnemy.isAttackReady = true;
            }
            //남은 거리가 사거리보다 짧을 경우 (원거리 공격)
            else if (dist < selectEnemy.range)
            {
                //사거리 만큼 거리를 둔다.
                int temp = selectEnemy.range - dist;

                path.RemoveRange(selectEnemy.moveCount - temp, dist + temp);
                selectEnemy.isAttackReady = true;
            }
            //타겟과의 거리가 사거리보다 길다면
            else if (dist > selectEnemy.range)
            {
                path.RemoveRange(selectEnemy.moveCount, dist);
            }
        }
        //이동 거리 내에 타겟이 있다면
        else
        {
            path.RemoveRange(path.Count - selectEnemy.range, selectEnemy.range);
            selectEnemy.isAttackReady = true;
        }

        selectEnemy.OnTileSet(path.Last());
        //이동 함수를 호출하고, 타일 위에 적을 세팅
        selectEnemy.MoveTile();
        path.Last().onObject = OnObject.ENEMY;
        path.Last().OnEnemy = selectEnemy;
    }
    //행동을 하지 않은 다음 적 플레이
    public void NextEnemyPlay()
    {
        GameUIManager.instance.inGamePnl.SetActive(false);

        selectEnemy.isTurnOver = true;
        //적 중 턴이 끝나지 않은 캐릭터를 selectEnemy로 세팅
        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i].isTurnOver == true)
            {
                continue;
            }
            //행동을 하지 않은 캐릭터 세팅 후 타겟 검색
            selectEnemy = enemies[i];
            FindPlayer();
            return;
        }
        //모든 오브젝트의 행동이 끝났다면 턴을 바꿔준다.
        GameManager.instance.TurnChange();
    }
}
