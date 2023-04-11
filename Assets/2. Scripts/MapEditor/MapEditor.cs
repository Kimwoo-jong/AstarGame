using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public static MapEditor instance;
    
    public GameObject mapTile;                      //맵에 배치할 타일
    private SpriteRenderer mapTileSprite;           //맵 타일 스프라이트 렌더러

    public int mapWidth = 0;                        //맵의 너비
    public int mapHeight = 0;                       //맵의 높이

    public GameObject[,] tileObjects;               //맵에 생성된 타일오브젝트 배열
    private Tile[,] tiles;                          //타일들의 Grid2d 컴포넌트 배열
    private GameObject selectedTile;                //선택된 타일 프리팹

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        tileObjects = new GameObject[mapWidth, mapHeight];
        tiles = new Tile[mapWidth, mapHeight];

        mapTileSprite = mapTile.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        SetMapTile();
    }
    //맵 타일을 생성해주는 함수
    public void SetMapTile()
    {
        for(int x = 0; x < mapWidth; ++x)
        {
            for(int y = 0 ; y < mapHeight; ++y)
            {
                tileObjects[x,y] = Instantiate(mapTile, new Vector3(x,y,0), Quaternion.identity);
                tileObjects[x,y].transform.parent = this.gameObject.transform;

                tiles[x,y] = tileObjects[x,y].GetComponent<Tile>();
                tiles[x,y].ID = -1;
            }
        }
        //카메라의 위치 조정(맵의 중간에 오도록)
        Camera.main.transform.position = new Vector3(mapWidth / 2, mapHeight / 2, -10);
    }
    //이미지를 Default로 변경해주는 함수
    public void ChangeBasicImage()
    {
        foreach(var tile in tiles)
        {
            tile.spriteRenderer.sprite = mapTileSprite.sprite;
        }
    }
    //세이브 전, 타일 생성 및 세팅
    public void MakeTiles()
    {
        for(int x =0;x<mapWidth;++x)
        {
            for(int y=0;y<mapHeight;++y)
            {
                Destroy(tileObjects[x,y]);
                
                tileObjects[x,y] = Instantiate(EditorUIManager.instance.tilePrefabs[tiles[x,y].ID],
                                                tiles[x,y].transform.position, tiles[x,y].transform.rotation);

                tileObjects[x,y].SetActive(true);
                tileObjects[x,y].transform.parent = this.gameObject.transform;
            }
        }
    }
}