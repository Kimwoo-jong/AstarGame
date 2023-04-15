using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject startPos;                         //이동시킬 리소스 객체
    private GameObject[] startPoints;                   //스타팅 포인트

    public StartPos selectStartPos { get; set; }        //선택된 스타팅 포인트

    private void Start()
    {
        int len = TempMap.instance.startTile.Length;
        startPoints = new GameObject[len];

        //자식 오브젝트를 담아준다.
        for (int i = 0; i < len; ++i)
        {
            startPoints[i] = Instantiate(startPos);
            startPoints[i].transform.SetParent(transform);

            int x = TempMap.instance.startTile[i].tileX;
            int y = TempMap.instance.startTile[i].tileY;
            startPoints[i].GetComponent<StartPos>().SetPosition(x, y);
        }

        selectStartPos = transform.GetChild(0).GetComponent<StartPos>();
    }
}