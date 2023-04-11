using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private Vector3 mouseOriginPos;
    private bool isPush;

    private void Update()
    {
        //마우스 우측 버튼 클릭 시
        if(Input.GetMouseButtonDown(1))
        {
            mouseOriginPos = Input.mousePosition;
            isPush = true;
        }
        //마우스 우측 버튼에서 손을 뗐을 시
        if(Input.GetMouseButtonUp(1))
        {
            isPush = false;
        }

        if(!isPush)
        {
            return;
        }
        //마우스 우측 버튼을 누르면 카메라 이동이 가능
        Vector3 pos = Camera.main.ScreenToViewportPoint(mouseOriginPos - Input.mousePosition);
        Vector3 cameraMove = new Vector3(pos.x * moveSpeed, pos.y * moveSpeed, 0);
        transform.Translate(cameraMove);
    }
}
