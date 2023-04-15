using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPos : MonoBehaviour
{
    public PlayerFace selectFace { get; set; }

    private CanvasController canvasController;
    private SelectPlayer selectPlayer;
    private RectTransform rt;

    public int posX { get; set; }
    public int posY { get; set; }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    private void Start()
    {
        canvasController = transform.parent.GetComponent<CanvasController>();
        selectPlayer = GameObject.Find("Contents").GetComponent<SelectPlayer>();
    }
    public void ClickStartPos()
    {
        //선택된 타일을 자기 자신으로
        canvasController.selectStartPos = this;
    }
    //월드맵에 위치 세팅 및 포지션 값 세팅
    public void SetPosition(int x , int y)
    {
        rt.localPosition = new Vector3(x, y, 0);
        posX = x;
        posY = y;
    }
}
