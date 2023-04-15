using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraFollow : MonoBehaviour
{
    private Vector2 prevPos = Vector2.zero;             //카메라의 이전 위치

    Vector2 targetPos = Vector2.zero;                   //카메라에 비춰질 타겟

    public float moveSpeed = 1.5f;                      //카메라 이동속도
    public float zoomSpeed = 0.01f;                     //카메라 줌 속도
    private float lerpSpeed = 10f;                      //자연스럽게 변환시켜주기 위한 Lerp 속도

    public Vector2 maxXandY;                            //카메라가 움직일 거리의 최대값
    public Vector2 minXandY;                            //카메라가 움직일 거리의 최소값

    private float minX, minY, maxX, maxY;               //카메라의 꼭지점 체크 float 변수
    private float tileSize;                             //맵의 크기

    public bool isDrag { get; set; }                    //드래그 중인지 확인하기 위한 변수

    private void Start()
    {
        Tile[,] tiles = TempMap.instance.tiles;
        tileSize = tiles[0, 0].TileSize;
        int lengthX = TempMap.instance.mapWidth - 1;
        int lengthY = TempMap.instance.mapHeight - 1;

        maxXandY = new Vector2(tiles[lengthX, 0].transform.position.x, tiles[0, lengthY].transform.position.y);
        minXandY = new Vector2(tiles[0, 0].transform.position.x, tiles[0, 0].transform.position.y);

        transform.position = new Vector3((lengthX + 1) / 2, (lengthY + 1) / 2, transform.position.z);
    }

    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS)

        DragMoveAndZoom();
#endif
    }

    private void DragMoveAndZoom()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }

        //드래그로 맵 이동
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                prevPos = touch.position - touch.deltaPosition;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                isDrag = true;

                Vector2 nowPos = touch.position - touch.deltaPosition;
                Vector2 movePos = (prevPos - nowPos) * moveSpeed * Time.deltaTime;

                transform.Translate(movePos);
                //MoveLimit();
                prevPos = touch.position - touch.deltaPosition;
            }
        }

        //줌인 줌아웃
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);//첫번째 손가락 좌표
            Touch touchOne = Input.GetTouch(1); //두번째 손가락 좌표

            //deltaPosition은 deltaTime과 동일하게 delta만큼 시간동안 움직인 거리
            //현재 position에서 이전 delta값 빼주면 움직이기전의 손가락 위치
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            //현재와 과거 움직임의 크기를 구한다.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            //두 값의 차이는 확대/축소 할 때 얼마만큼 진행되는지 결정
            float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;

            //줌
            Camera.main.orthographicSize += deltaMagDiff * zoomSpeed * Time.deltaTime;

            //줌 크기 최대 최소 세팅
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, 7f);
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 3f);

            MoveLimit();
        }

    }

    private void LateUpdate()
    {
        if (GameManager.instance.isStart == false)
        {
            return;
        }

        TrackTarget();
    }

    private void TrackTarget()
    {
        if (GameManager.instance.isPlayerTurn)
        {
            if (isDrag)
            {
                return;
            }

            //플레이어 선공에서 플레이어 죽을 시 에러 방지
            if (PlayerManager.instance.selectPlayer == null)
            {
                return;
            }

            targetPos = new Vector2(
                PlayerManager.instance.selectPlayer.transform.position.x,
                PlayerManager.instance.selectPlayer.transform.position.y);
        }
        else
        {
            //적 선공에서 적이 죽을 시 에러 방지   
            if (EnemyManager.instance.selectEnemy == null)
            {
                return;
            }

            targetPos = new Vector2(
                EnemyManager.instance.selectEnemy.transform.position.x,
                EnemyManager.instance.selectEnemy.transform.position.y);
        }

        float targetX = targetPos.x;
        float targetY = targetPos.y;

        //카메라 사각 꼭지점 위치 받아오기
        minX = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.5f, 0f)).x;
        maxX = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, 0f)).x;
        minY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, 0f)).y;
        maxY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.0f, 0f)).y;

        float halfLengthX = Mathf.Abs((maxX - minX) / 2);
        float halfLengthY = Mathf.Abs((maxY - minY) / 2);

        // 카메라가 맵을 빠져나가지 못하도록
        targetX = Mathf.Clamp(targetX, minXandY.x + halfLengthX - tileSize / 2, maxXandY.x - halfLengthX + tileSize / 2);
        targetY = Mathf.Clamp(targetY, minXandY.y + halfLengthY - tileSize / 2, maxXandY.y - halfLengthY + tileSize / 2);

        //카메라 자연스럽게 이동
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, targetY, transform.position.z), Time.deltaTime * lerpSpeed);
    }
    //이동 위치 제한을 걸어줌
    private void MoveLimit()
    {
        Vector3 temp;

        minX = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.5f, 0f)).x;
        maxX = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, 0f)).x;
        minY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, 0f)).y;
        maxY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.0f, 0f)).y;

        float halfLengthX = Mathf.Abs((maxX - minX) / 2);
        float halfLengthY = Mathf.Abs((maxY - minY) / 2);

        temp.x = Mathf.Clamp(minX, minXandY.x + halfLengthX - tileSize / 2, maxXandY.x - halfLengthX + tileSize / 2);
        temp.y = Mathf.Clamp(minY, minXandY.y + halfLengthY - tileSize / 2, maxXandY.y - halfLengthY + tileSize / 2);
        temp.z = -10;

        transform.position = temp;
    }
}