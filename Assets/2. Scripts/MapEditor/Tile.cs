using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None,
    Player = 40,
    Enemy = 100,
    Item = 1000
}

public class Tile : MonoBehaviour
{
    public int ID { get; set; }                                         //타일의 ID
    public bool isWalkable { get; set; }                                //걸을 수 있는 곳인지 여부

    public int gridSizeX { get; set; }
    public int gridSizeY { get; set; }                                  //그리드 기준 좌표

    private TileType tileType;                                          //오브젝트의 타입
    
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Player onPlayer { get; set; }              //플레이어
    [HideInInspector] public Player onEnemy { get; set; }               //상대

    public int fCost { get; set; }
    public int gCost { get; set; }
    public int hCost { get; set; }

    public Tile nextTile { get; set; }                                  //부모노드를 담아두기 위함

    private void Awake()
    {
        switch (this.gameObject.tag)
        {
            case "WalkableTile":
                isWalkable = true;
                break;
            case "NonWalkableTile":
                isWalkable = false;
                break;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}