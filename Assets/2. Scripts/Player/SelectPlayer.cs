using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectPlayer : MonoBehaviour
{
    public GameObject contents;                        //플레이어 초상화를 담기 위한 변수
    public GameObject portraitPrefab;                  //플레이어 초상화 오브젝트

    private GameObject[] resources;
    private GameObject[] players;
    private GameObject[] faces;

    Tile[,] tiles;

    private CanvasController canvasControl;
    private void Awake()
    {
        resources = Resources.LoadAll<GameObject>("Players");
        players = new GameObject[resources.Length];
        faces = new GameObject[players.Length];
        canvasControl = GameObject.Find("GameCanvas").GetComponent<CanvasController>();
    }

    private void Start()
    {
        for (int i = 0; i < resources.Length; ++i)
        {
            players[i] = Instantiate(resources[i]);
            faces[i] = Instantiate(portraitPrefab);
            faces[i].GetComponent<PlayerFace>().Player = players[i].GetComponent<Player>();
            faces[i].GetComponent<Image>().sprite = players[i].transform.Find("face").GetComponent<SpriteRenderer>().sprite;
            players[i].SetActive(false);
            faces[i].transform.SetParent(contents.transform);
            faces[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }

        tiles = TempMap.instance.tiles;
    }

    //플레이어 타일 위에 소환
    public void SetPlayerOnTile(Player player, int X, int Y)
    {
        //플레이어가 이미 세팅되어 있다면
        if (tiles[X, Y].OnPlayer != null)
        {
            tiles[X, Y].OnPlayer.gameObject.SetActive(false);
        }
        player.gameObject.SetActive(true);
        player.SetPlayerPosition(tiles[X, Y]);
    }
    public void SetSelectFace(PlayerFace face)
    {
        //초상화가 겹친다면
        if (canvasControl.selectStartPos.selectFace != null)
        {
            //기존 골라져있는 캐릭터 원상복귀시키고
            canvasControl.selectStartPos.selectFace.gameObject.SetActive(true);
            canvasControl.selectStartPos.selectFace.transform.SetParent(transform);

            //현재 선택된 스타트포지션의 캐릭터face 세팅
            canvasControl.selectStartPos.selectFace = face;

            //셀렉창에서 안보이게 부모날려주기
            face.transform.parent = null;
        }
        //새로 들어온 페이스면
        else
        {
            //현재 선택된 스타트포지션의 캐릭터face 세팅
            canvasControl.selectStartPos.selectFace = face;

            face.transform.parent = null;
            face.gameObject.SetActive(false);
        }

        //실제 인게임캐릭터 배치
        int x = canvasControl.selectStartPos.posX;
        int y = canvasControl.selectStartPos.posY;

        SetPlayerOnTile(face.Player, x, y);
    }
}
