using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OnObject { NONE, PLAYER, ENEMY, ITEM }

public class Tile : MonoBehaviour {

    //타일 고유 id
    public int ID { get; set; }

    public bool Walkable { get; set; }

    //tiles 기준 index
    public int tileX { get; set; }
    public int tileY { get; set; }

    //tile사이즈 (월드좌표기준 크기)
    public float TileSize { get; set; }
        
    [HideInInspector] public SpriteRenderer spRenderer;

    //타일 위 오브젝트 상태
    [HideInInspector] public OnObject onObject = OnObject.NONE;
    [HideInInspector] public Player OnPlayer { get; set; }
    [HideInInspector] public Player OnEnemy { get; set; }

    //astar
    public int fCost { get; set; }                              //g + h (제일 낮은 것 부터 길찾기 검색)
    public int gCost { get; set; }                              //gCost(현재 닫힌 목록에 있는 타일과의 거리)
    public int hCost { get; set; }                              //hCost(목적지 까지의 거리 [장애물 고려 X])

    public Tile NextTile { get; set; }                      //astar에서 부모타일 담아두기용

    private void Awake()
    {
        switch (this.gameObject.tag)
        {
            case "WalkableTile":
                Walkable = true;
                break;
            case "NonWalkableTile":
                Walkable = false;
                break;
        }
        spRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}