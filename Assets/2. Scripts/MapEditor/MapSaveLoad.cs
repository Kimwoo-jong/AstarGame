using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSaveLoad : MonoBehaviour
{
    string filePath;                            //파일의 경로

    private void Start()
    {
        MapEditor map = new MapEditor();
        //Application.dataPath : 프로젝트디렉토리/Assets
        //.persistent : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름
        //.streamingAssetsPath : 프로젝트디렉토리/Assets/StreamingAssets
        filePath = Application.dataPath + "/MapFile/map.bin";
    }

    //바이너리 파일 세이브
    public void WriteToBinaryFile<T>(string filePath, T objectToWrite)
    {
        BinaryFormatter format = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Create);

        format.Serialize(fileStream, objectToWrite);
        fileStream.Close();
    }
    //바이너리 파일 로드
    public T ReadFromBinaryFile<T>(string filePath)
    {
        BinaryFormatter format = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Open);

        T temp = (T)format.Deserialize(fileStream);
        fileStream.Close();

        return temp;
    }
    //맵 세이브하기
    public void SaveMap()
    {
        MapEditor.instance.MakeTiles();
        WriteToBinaryFile<int>(filePath, MapEditor.instance.mapWidth);
        WriteToBinaryFile<int>(filePath, MapEditor.instance.mapHeight);
    }
    //맵 로드하기
    public void LoadMap()
    {
        //파일 경로에 파일이 있을 때
        if(System.IO.File.Exists(filePath))
        {
            MapEditor map = ReadFromBinaryFile<MapEditor>(filePath);
        }
    }
}
