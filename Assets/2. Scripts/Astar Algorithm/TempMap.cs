using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//레벨 에디터 기능이 구현되기 전에 사용할 임시맵
public class TempMap : MonoBehaviour
{
    public static TempMap instance;

    //임시 변수
    private GameObject[] walkableTiles;                 //걸을 수 있는 타일 프리팹
    private GameObject[] waterTiles;                    //물 웅덩이 프리팹

    public GameObject waterPool;                        //물 프리팹

    public int mapWidth = 10;                           //타일맵의 길이
    public int mapHeight = 10;                          //타일맵의 너비

    public GameObject[,] tileObjects;                   //맵에 있는 타일들의 게임오브젝트 배열
    public Tile[,] tiles;                               //맵에 있는 타일들의 타일 컴포넌트 배열

    private List<Tile> moveTiles;                       //거리 상으로 이동 가능한 모든 타일 리스트
    public List<Tile> atkTiles;                         //공격 가능한 Enemy 타일 리스트

    [HideInInspector] public Tile[] startTile;          //플레이어의 시작 위치

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        tileObjects = new GameObject[mapWidth, mapHeight];
        tiles = new Tile[mapWidth, mapHeight];
        moveTiles = new List<Tile>();
        atkTiles = new List<Tile>();

        walkableTiles = new GameObject[9];
        for (int i = 0; i < walkableTiles.Length; i++)
        {
            string name = string.Format("Tiles/roadTile{0}", i + 1);
            walkableTiles[i] = Resources.Load(name) as GameObject;
        }
        waterTiles = new GameObject[4];
        for (int i = 0; i < waterTiles.Length; i++)
        {
            string name = string.Format("Tiles/waterTile{0}", (i + 1) * 3);
            waterTiles[i] = Resources.Load(name) as GameObject;
        }
        //맵 전체에 이동할 수 있는 타일을 먼저 깔아줌
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                tileObjects[x, y] = Instantiate(walkableTiles[Random.Range(0, walkableTiles.Length)],
                                                            new Vector3(x, y, 0), Quaternion.identity);
                //TempMap 스크립트가 있는 오브젝트를 부모로 설정
                tileObjects[x, y].transform.SetParent(this.transform);
                tiles[x, y] = tileObjects[x, y].GetComponent<Tile>();

                tiles[x, y].tileX = x;
                tiles[x, y].tileY = y;
                tiles[x, y].TileSize = 1;
            }
        }
        //맵 중간 중간에 물을 깔아줌.
        SetWaterPool(2, 3); SetWaterPool(2, 4); SetWaterPool(2, 7);
        SetWaterPool(3, 4);
        SetWaterTiles(14, 9);
        SetWaterTiles(8, 12);
        //스타트 지점 세팅
        startTile = new Tile[3] { tiles[11, 6], tiles[1, 5], tiles[5, 8] };

        ShowStartPoints();
    }
    //스타트 포인트를 보여주는 함수
    public void ShowStartPoints()
    {
        for (int i = 0; i < startTile.Length; ++i)
        {
            startTile[i].transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    public void UnShowStartPoints()
    {
        for (int i = 0; i < startTile.Length; ++i)
        {
            startTile[i].transform.GetChild(3).gameObject.SetActive(false);
        }
    }
    //물 이미지 하나씩 맵에 배치
    private void SetWaterPool(int x, int y)
    {
        Destroy(tileObjects[x, y]);

        tileObjects[x, y] = Instantiate(waterPool, new Vector3(x, y, 0), Quaternion.identity);
        tileObjects[x, y].transform.SetParent(this.transform);
        tiles[x, y] = tileObjects[x, y].GetComponent<Tile>();
    }
    //물 웅덩이를 타일 주변(자기 자신 위치 포함) 4개 배치
    private void SetWaterTiles(int x, int y)
    {
        //해당 위치를 기준으로 자기 자신 >> ㅁ ㅁ
        //                                ㅁ ㅁ 위치에 타일을 배치한다
        Destroy(tileObjects[x, y]);
        Destroy(tileObjects[x + 1, y]);
        Destroy(tileObjects[x, y - 1]);
        Destroy(tileObjects[x + 1, y - 1]);
        //위치에 맞춰서 생성해준다.
        tileObjects[x, y] = Instantiate(waterTiles[0], new Vector3(x, y, 0), Quaternion.identity);
        tileObjects[x, y - 1] = Instantiate(waterTiles[1], new Vector3(x, y - 1, 0), Quaternion.identity);
        tileObjects[x + 1, y] = Instantiate(waterTiles[2], new Vector3(x + 1, y, 0), Quaternion.identity);
        tileObjects[x + 1, y - 1] = Instantiate(waterTiles[3], new Vector3(x + 1, y - 1, 0), Quaternion.identity);
        //생성된 오브젝트의 부모를 Map 오브젝트로 설정
        tileObjects[x, y].transform.SetParent(this.transform);
        tileObjects[x, y - 1].transform.SetParent(this.transform);
        tileObjects[x + 1, y].transform.SetParent(this.transform);
        tileObjects[x + 1, y - 1].transform.SetParent(this.transform);
        //생성된 타일오브젝트의 Tile 컴포넌트 연결
        tiles[x, y] = tileObjects[x, y].GetComponent<Tile>();
        tiles[x, y - 1] = tileObjects[x, y - 1].GetComponent<Tile>();
        tiles[x + 1, y] = tileObjects[x + 1, y].GetComponent<Tile>();
        tiles[x + 1, y - 1] = tileObjects[x + 1, y - 1].GetComponent<Tile>();
    }
    //선택한 플레이어가 갈 수 있는 타일을 보여주는 함수
    public List<Tile> ShowWalkableTile(Tile tile, int count)
    {
        GameUIManager.instance.inGamePnl.SetActive(true);
        moveTiles.Clear();
        ClearTileColor();

        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                //이동할 수 없는 타일의 경우 무시하도록
                if (tiles[x, y].Walkable == false)
                {
                    continue;
                }

                //이동거리 내에 위치한 타일들 모두를 포함
                if (Mathf.Abs(tile.tileX - tiles[x, y].tileX) + Mathf.Abs(tile.tileY - tiles[x, y].tileY) <= count)
                {
                    //플레이어나 적이 있는 타일은 제외함
                    if ((tiles[x, y].onObject == OnObject.PLAYER) || (tiles[x, y].onObject == OnObject.ENEMY))
                    {
                        tiles[x, y].transform.GetChild(2).gameObject.SetActive(true);
                        continue;
                    }
                    //길찾기 시작
                    Astar.instance.SetStartnTargetIndex(tile.tileX, tiles[x, y].tileX, tile.tileY, tiles[x, y].tileY);
                    bool result = Astar.instance.PathFinding();

                    //길을 찾았다면
                    if (result)
                    {
                        int moveCount = Astar.instance.BestWayList.Count;

                        //갈 수 있는 거리가 이동 가능 거리보다 높다면
                        if (moveCount > count)
                        {
                            tiles[x, y].transform.GetChild(2).gameObject.SetActive(true);
                            continue;
                        }

                        moveTiles.Add(tiles[x, y]);
                        tiles[x, y].transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
                //이동 거리내에 있지 않다면
                else
                {
                    tiles[x, y].transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }

        return moveTiles;
    }
    //타일 선택 후 이동이 실행되면 사각형을 표시해주는 이미지 비활성화
    public void ClearTileColor()
    {
        for(int x = 0; x < mapWidth; ++x)
        {
            for(int y = 0; y < mapHeight; ++y)
            {
                if(tiles[x,y].Walkable == false)
                {
                    continue;
                }

                tiles[x, y].transform.GetChild(1).gameObject.SetActive(false);
                tiles[x, y].transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }
    //공격 가능한 타일을 보여주는 함수
    public void ShowAttackTile(Tile tile, Player player)
    {
        //초기화부터
        atkTiles.Clear();

        for(int x=0; x<mapWidth;++x)
        {
            for(int y=0;y<mapHeight;++y)
            {
                //Enemy가 올라와 있는 타일
                if(tiles[x,y].onObject == OnObject.ENEMY)
                {
                    //사정거리 내에 있다면
                    if(Mathf.Abs(tile.tileX - tiles[x,y].tileX) + Mathf.Abs(tile.tileY - tiles[x,y].tileY) <= player.range)
                    {
                        tiles[x, y].transform.GetChild(2).gameObject.SetActive(true);
                        atkTiles.Add(tiles[x, y]);
                    }
                }
            }
        }
    }
    //현재 위치부터 거리만큼 이동가능한 타일을 전부 받아오는 함수
    public List<Tile> GetRangeTile(Tile tile, int range)
    {
        moveTiles.Clear();

        for(int x=0; x<mapWidth;++x)
        {
            for(int y=0;y<mapHeight;++y)
            {
                if(tiles[x,y].Walkable == false)
                {
                    continue;
                }

                if (Mathf.Abs(tile.tileX - tiles[x, y].tileX) + Mathf.Abs(tile.tileY - tiles[x, y].tileY) == range)
                {
                    if(tiles[x,y].onObject == OnObject.PLAYER || tiles[x, y].onObject == OnObject.ENEMY)
                    {
                        continue;
                    }

                    Astar.instance.SetStartnTargetIndex(tile.tileX, tiles[x, y].tileX, tile.tileY, tiles[x, y].tileY);
                    bool result = Astar.instance.PathFinding();

                    if(result)
                    {
                        moveTiles.Add(tiles[x, y]);
                    }
                }
            }
        }

        return moveTiles;
    }
}