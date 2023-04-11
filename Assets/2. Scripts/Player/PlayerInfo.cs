using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    //플레이어의 직업 확인을 위한 프로퍼티
    protected PlayerClass playerClass;
    public PlayerClass _playerClass
    {
        get
        {
            return playerClass;
        }
        set
        {
            playerClass = value;
        }
    }

    //플레이어의 스탯
    public int MaxHP { get; set; }
    public int curHP { get; set; }
    public int Atk { get; set; }
    public int def { get; set; }
    public int Range { get; set; }

    //이동 가능한 타일의 수
    public int movableTileCount { get; set; }
    //플레이어 이름
    public string name { get; set; }
}