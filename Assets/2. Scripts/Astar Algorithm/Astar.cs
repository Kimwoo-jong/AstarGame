using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//방향 체크를 위한 구조체
[System.Serializable]
public struct Direction
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Weight { get; set; }

    public Direction(int x, int y, int weight)
    {
        this.X = x;
        this.Y = y;
        this.Weight = weight;
    }
}

public class Astar : MonoBehaviour
{
    public static Astar instance;

    public TempMap map;
    private Tile[,] tiles;

    private Tile startTile;                         //시작점
    private Tile targetTile;                        //목적지

    private int mapWidth;
    private int mapHeight;

    //Astar 알고리즘 사용을 위한 리스트
    private List<Tile> openList;                    //열린 리스트(갈 수 있는 길)
    private List<Tile> closeList;                   //닫힌 리스트(이미 지나온 길)
    private List<Tile> bestWayList;                 //최단 거리

    public List<Tile> BestWayList
    {
        get
        {
            return bestWayList;
        }
    }
    //상하좌우 방향
    private Direction[] directions =
    {
        new Direction(1,0,10),
        new Direction(-1,0,10),
        new Direction(0,1,10),
        new Direction(0,-1,10)
    };

    private bool isSuccess = false;                 //길찾기 성공 실패 확인

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //타일 초기화
        startTile = null;
        targetTile = null;
    }

    private void Start()
    {
        //맵의 복사본을 만들어 저장
        tiles = (Tile[,])map.tiles.Clone();

        openList = new List<Tile>();
        closeList = new List<Tile>();
        bestWayList = new List<Tile>();
        //맵의 크기도 복사해둠
        mapWidth = TempMap.instance.mapWidth;
        mapHeight = TempMap.instance.mapHeight;
    }
    //시작지점과 목표지점의 인덱스 설정
    public void SetStartnTargetIndex(int x1, int x2, int y1, int y2)
    {
        startTile = tiles[x1, y1];
        targetTile = tiles[x2, y2];
    }
    //리스트 초기화 함수
    public void ResetList()
    {
        openList.Clear();
        closeList.Clear();
        bestWayList.Clear();
    }
    //길찾기 함수
    public bool PathFinding()
    {
        ResetList();
        isSuccess = SetTile();
        SetPathList();

        return isSuccess;
    }
    public bool SetTile()
    {
        Tile init = tiles[startTile.tileX, startTile.tileY];

        init.gCost = 0;
        init.hCost = Mathf.Abs(targetTile.tileX - init.tileX) + Mathf.Abs(targetTile.tileY - init.tileY);
        init.fCost = init.gCost + init.hCost;

        //타일의 코스트 체크 후 시작지점에 연결
        tiles[startTile.tileX, startTile.tileY] = init;

        openList.Add(init);

        //갈 수 있는 길들이 있다면
        while (openList.Count > 0)
        {
            Tile tile = openList[0];

            for (int i = 0; i < openList.Count; ++i)
            {
                if (openList[i].fCost < tile.fCost)
                {
                    tile = openList[i];
                }
            }
            //목적지를 찾았다면 멈춤
            if (tile.tileX == targetTile.tileX && tile.tileY == targetTile.tileY)
            {
                break;
            }
            //목적지가 아닐 경우 열린 리스트에서 제거
            //닫힌 리스트에 추가하고 주변을 검색
            openList.Remove(tile);
            closeList.Add(tile);
            AddNeighbours(tile);
        }
        //길이 없을 경우
        if (openList.Count == 0)
        {
            Debug.Log("갈 수 있는 길이 없습니다.");
            return false;
        }

        return true;
    }
    //타일 기준으로 주변 타일들 확인
    public void AddNeighbours(Tile currTile)
    {
        for (int i = 0; i < directions.Length; ++i)
        {
            int tempX = currTile.tileX + directions[i].X;
            int tempY = currTile.tileY + directions[i].Y;

            if (tempX < 0 || tempY < 0 || tempX >= mapWidth || tempY >= mapHeight)
            {
                continue;
            }

            Tile tile = tiles[tempX, tempY];
            //이미 지나왔거나, 걸을 수 없는 곳이라면
            if (closeList.Contains(tile) || tile.Walkable == false)
            {
                continue;
            }

            //열린 목록에 없는 경우
            if (!openList.Contains(tile))
            {
                tile.gCost = currTile.gCost + directions[i].Weight;
                tile.hCost = Mathf.Abs(targetTile.tileX - tile.tileX) + Mathf.Abs(targetTile.tileY - tile.tileY);
                tile.fCost = tile.gCost + tile.hCost;
                tile.NextTile = currTile;

                tiles[tempX, tempY] = tile;
                openList.Add(tile);
            }
            //이미 열린 목록에 있을 경우
            //이전 부모의 gCost와 현재 부모의 gCost를 비교하여 작을 경우에 업데이트
            else if (tiles[tempX, tempY].gCost > currTile.gCost + 10)
            {
                tile.gCost = currTile.gCost + directions[i].Weight;
                tile.fCost = tile.gCost + tile.hCost;
                tile.NextTile = currTile;
            }
        }
    }
    //경로를 세팅
    public void SetPathList()
    {
        Tile tile = tiles[targetTile.tileX, targetTile.tileY];

        while (tile != null)
        {
            //목적지를 리스트에 추가한 뒤
            bestWayList.Add(tile);

            //부모 타일을 찾아서 거슬러 올라간다.
            if (tile.NextTile != null)
            {
                //출발지일 경우 멈춘다.
                if (tile.NextTile.Equals(startTile))
                {
                    break;
                }
            }

            tile = tile.NextTile;
        }
        //리스트를 반대로 하면 목적지까지 순서대로 정렬
        bestWayList.Reverse();
    }
}