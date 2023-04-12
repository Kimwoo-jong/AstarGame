using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private TileMap2D tilemap2D;           //맵 크기 정보를 얻어오기 위함.
    private Camera mainCam;                                 //카메라 시야 조절을 위한 Camera

    private float wDelta = 0.4f;                            //Width 시야 delta값
    private float hDelta = 0.6f;                            //Height 시야 delta값

    [SerializeField] private float moveSpeed;               //카메라 이동 속도
    [SerializeField] private float zoomSpeed;               //카메라 줌 속도
    [SerializeField] private float minViewSize;             //카메라 시야 최소 크기
    private float maxViewSize;                              //카메라 시야 최대 크기

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
    }
    //맵 생성 버튼을 눌렀을 때 카메라의 위치 설정을 위함
    public void SetupCamera()
    {
        //맵 크기 정보
        int width = tilemap2D.Width;
        int height = tilemap2D.Height;

        //카메라 시야 설정(전체 맵이 화면에 들어올 수 있도록 함)
        float size = (width > height) ? width * wDelta : height * hDelta;
        mainCam.orthographicSize = size;

        //카메라의 위치 설정 (Y축 좌표)
        if(height > width)
        {
            //높이가 더 클 경우 Y축 수정
            Vector3 pos = new Vector3(0, 0.05f, -10);
            pos.y *= height;

            transform.position = pos;
        }

        maxViewSize = mainCam.orthographicSize;
    }
    //카메라의 이동 위치 설정
    public void SetPosition(float x, float y)
    {
        transform.position += new Vector3(x,y,0) * moveSpeed * Time.deltaTime;
    }
    //카메라의 줌
    public void SetOrthographicSize(float size)
    {
        if(size == 0)
        {
            return;
        }

        //카메라의 시야 설정
        mainCam.orthographicSize += size * zoomSpeed * Time.deltaTime;
        //카메라의 시야 범위는 (min <= orthographicSize <= max)
        mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minViewSize, maxViewSize);
    }
}
