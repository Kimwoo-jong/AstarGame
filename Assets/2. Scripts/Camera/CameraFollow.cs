using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraFollow : MonoBehaviour
{
    private Vector2 prevPos = Vector2.zero;             //카메라의 이전 위치
    private float prevDist = 0f;

    Vector2 targetPos = Vector2.zero;                   //카메라에 비춰질 타겟

    public float moveSpeed = 1.5f;                      //카메라 이동속도
    public float zoomSpeed = 0.01f;                     //카메라 줌 속도
    private float lerpSpeed = 10f;                      //자연스럽게 변환시켜주기 위한 Lerp 속도

    private float minX, minY, maxX, maxY;               //카메라의 꼭지점 체크 float 변수
    private float tileSize;                             //맵의 크기

    [SerializeField] private bool isDrag;               //드래그 중인지 확인하기 위한 변수

    private void Start()
    {
        
    }
}
