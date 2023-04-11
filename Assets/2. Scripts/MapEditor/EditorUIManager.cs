using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorUIManager : MonoBehaviour
{
    public static EditorUIManager instance;
    
    private Vector3 mousePos;

    public GameObject buttonPanel;
    public Button buttonTile;

    //타일 프리팹 딕셔너리
    //Key와 Value로 원하는 정보를 편리하게 가져올 수 있음.
    public Dictionary<int, GameObject> tilePrefabs;
}
