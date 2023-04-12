// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;

// public class EditorUIManager : MonoBehaviour
// {
//     public static EditorUIManager instance;                 //싱글톤 사용
    
//     private Vector3 mousePos;                               //마우스의 위치

//     public GameObject buttonPanel;
//     public Button buttonTile;

//     //타일 프리팹 딕셔너리
//     //Key와 Value로 원하는 정보를 편리하게 가져올 수 있음.
//     public Dictionary<int, GameObject> tilePrefabs;

//     private List<Button> tileButtons;                       //타일의 갯수만큼 버튼을 생성하기 위함
    
//     public Image selectedTile;                              //선택된 타일 이미지
//     private Sprite savedSprite;                             //저장을 위한 스프라이트

//     private int tileID = 0;                                 //타일 ID
//     private int selectedTileID = 0;                         //선택된 타일의 ID

//     private void Awake()
//     {
//         if(instance == null)
//         {
//             instance = this;
//         }

//         tilePrefabs = new Dictionary<int, GameObject>();
//         tileButtons = new List<Button>();
//         //Resources/Tiles 폴더에 있는 모든 프리팹을 가져와서 저장
//         GameObject[] resourceTiles = Resources.LoadAll<GameObject>("Tiles");

//         foreach(GameObject obj in resourceTiles)
//         {
//             //Key : 타일 ID, Value : 타일 객체
//             tilePrefabs.Add(tileID++, Instantiate(obj));
//         }
//         tileID = 0;
//     }

//     private void Start()
//     {
//         //Dictionary<Key, value>를 foreach문에서 사용하려면
//         //KeyValuePair<Key, Value> 구조체를 사용하면 된다.
//         foreach(KeyValuePair<int, GameObject> tile in tilePrefabs)
//         {
//             //Tile의 ID 세팅
//             tile.Value.GetComponent<Tile>().ID = tileID++;
//             //생성된 타일은 비활성화
//             tile.Value.SetActive(false);

//             //생성된 타일 버튼을 패널의 자식으로 보내주기 위함
//             Button buttonChild = Instantiate(buttonTile);
//             buttonChild.transform.SetParent(buttonPanel.transform);
//             //buttonChild.transform.localScale = Vector3.one;
//             //버튼의 이미지를 타일 이미지로 변경
//             buttonChild.GetComponent<Image>().sprite = tile.Value.GetComponent<Tile>().spriteRenderer.sprite;
//             //버튼 리스트에 추가
//             tileButtons.Add(buttonChild);
//         }

//         SelectTileButton();
//     }

//     private void Update()
//     {
//         //마우스 좌클릭 시
//         if(Input.GetMouseButton(0))
//         {
//             //마우스가 UI위에 있다면 오브젝트 선택이 되지 않도록 반환
//             if(EventSystem.current.IsPointerOverGameObject() == true)
//             {
//                 return;
//             }

//             //마우스의 포지션을 받아온 뒤
//             //월드포지션으로 변환
//             mousePos = Input.mousePosition;
//             mousePos = Camera.main.ScreenToWorldPoint(mousePos);

//             //카메라로부터 스크린으로의 점
//             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

//             if(hit.collider != null)
//             {
//                 //선택한 타일이 존재한다면
//                 if(selectedTile != null)
//                 {
//                     Tile tempTile = hit.collider.gameObject.GetComponent<Tile>();
//                     tempTile.ID = selectedTileID;
//                     tempTile.spriteRenderer.sprite = savedSprite;
//                 }
//             }
//         }
//     }
//     //타일 버튼을 클릭하면 실행되는 함수
//     public void SelectTileButton()
//     {
//         for(int i = 0; i < tileButtons.Count;++i)
//         {
//             Tile tile = tilePrefabs[i].GetComponent<Tile>();
//             tileButtons[i].onClick.AddListener(() => ClickTile(tile.ID));
//         }
//     }
    
//     public void ClickTile(int id)
//     {
//         savedSprite = tilePrefabs[id].GetComponent<Tile>().spriteRenderer.sprite;
//         selectedTile.sprite = savedSprite;
//         selectedTileID = id;
//         Debug.Log("해당 타일의 ID는 " + id + "입니다.");
//     }
// }