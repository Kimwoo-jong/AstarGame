using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using TMPro;

public class MapDataSave : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFileName;
    [SerializeField] private TileMap2D tileMap2D;

    private void Awake()
    {
        inputFileName.text = "Noname.json";
    }
    public void Save()
    {
        //맵 크기, 맵에 있는 타일의 정보를 가져옴
        MapData mapData = tileMap2D.GetMapData();
        //인풋 필드에 입력된 텍스트의 정보를 파일 이름으로 저장
        string fileName = inputFileName.text;
        //파일 이름에 .json이라는 것이 없다면 추가해줌.
        if(fileName.Contains(".json") == false)
        {
            fileName += ".json";
        }
        //파일의 경로, 파일의 이름을 하나로 합칠 때 사용
        fileName = Path.Combine("MapData/",fileName);
        //mapData 인스턴스에 있는 내용을 직렬화하여 문자열 형태로 저장
        string toJson = JsonConvert.SerializeObject(mapData, Formatting.Indented);
        //해당 파일에 toJson의 내용을 저장
        File.WriteAllText(fileName, toJson);
    }
}
