using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : PlayerInfo
{
    private void Awake()
    {
        _playerClass = PlayerClass.ARCHER;
        MaxHP = 100;
        curHP = MaxHP;
        Atk = 40;
        def = 10;
        Range = 2;
        movableTileCount = 4;
        _name = "구우웅수";
    }
}
