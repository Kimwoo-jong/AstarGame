using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ObjectType
{
    None,
    Player,
    Enemy,
    Item
}

public class Tile : MonoBehaviour
{
    public Vector2 gridWorldSize;                       //그리드의 크기
    public Color color = Color.white;                   //기즈모의 색상
    public float nodeRadius;                            //노드의 크기

    float nodeDiameter;

    public GameObject[] prefabsList;                    //맵에 깔아둘 타일 프리팹 배열

    public int ID { get; set; }                         //타일의 ID
    public bool isWalkable;                             //걸을 수 있는 곳인지 여부

    public int gridSizeX, gridSizeY;                    //그리드 기준 좌표

    [HideInInspector] public SpriteRenderer spriteRenderer;               //스프라이트 렌더러
    [HideInInspector] public ObjectType objectType = ObjectType.None;     //오브젝트의 타입

    [HideInInspector] public Player onPlayer { get; set; }                //플레이어
    [HideInInspector] public Player onEnemy { get; set; }                 //상대

    public int fCost { get; set; }
    public int gCost { get; set; }
    public int hCost { get; set; }

    public Tile nextTile { get; set; }                                    //부모노드를 담아두기 위함

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