using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileMap2D : MonoBehaviour
{
    public GameObject basicTile;                                    //기본 타일
    private SpriteRenderer basicSprite;                             //기본 타일 스프라이트 렌더러
    private GameObject selectedTile;                                //선택한 타일 프리팹

    public GameObject[,] tileObjects;                               //맵에 배치되는 타일 배열
    private Tile[,] tiles;                                          //맵에 배치된 타일의 Tile 컴포넌트 배열

    public int Width { get; private set; } = 20;                    //맵의 X 크기
    public int Height { get; private set; } = 20;                   //맵의 Y 크기

    [SerializeField] private TMP_InputField inputWidth;             //맵의 Width 크기를 얻어올 Input Field
    [SerializeField] private TMP_InputField inputHeight;            //맵의 Heignt 크기를 얻어올 Input Field

    private void Awake()
    {
        inputWidth.text = Width.ToString();
        inputHeight.text = Height.ToString();

        basicSprite = basicTile.GetComponentInChildren<SpriteRenderer>();
        tileObjects = new GameObject[Width, Height];
        tiles = new Tile[Width, Height];
    }
    //기본 타일맵 생성
    public void GenerateTilemap()
    {
        //프로퍼티 사용을 위해 지역변수 선언
        int width, height;

        //Input Field에 있는 문자열을 변수에 정수로 저장
        int.TryParse(inputWidth.text, out width);
        int.TryParse(inputHeight.text, out height);

        Width = width;
        Height = height;

        for (int x = 0; x < Width; ++x)
        {
            for (int y = 0; y < Height; ++y)
            {
                //생성되는 맵의 중앙이 원점이 되도록
                Vector3 pos = new Vector3((-Width * 0.5f + 0.5f) + x, (Height * 0.5f - 0.5f) - y, 0);

                tileObjects[x,y] = Instantiate(basicTile, pos, Quaternion.identity);
                //자기 자신을 부모로 설정
                tileObjects[x,y].transform.SetParent(this.gameObject.transform);
                tiles[x,y] = tileObjects[x,y].GetComponent<Tile>();
            }
        }
    }
    //깔아둔 타일을 지우고 기본 이미지로 변경
    public void EraseTileMap()
    {
        foreach(var obj in tiles)
        {
            obj.spriteRenderer.sprite = basicSprite.sprite;
        }
    }
}