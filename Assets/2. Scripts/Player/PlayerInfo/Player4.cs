using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player4 : PlayerInfo
{
    private void Awake()
    {
        _playerClass = PlayerClass.ARCHER;
        MaxHP = 100;
        curHP = MaxHP;
        Atk = 35;
        def = 10;
        Range = 3;
        movableTileCount = 4;
        _name = "나이든 궁병";
    }
}
