using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : PlayerInfo
{
    private void Awake()
    {
        _playerClass = PlayerClass.WARRIOR;
        MaxHP = 100;
        curHP = MaxHP;
        Atk = 40;
        def = 15;
        Range = 1;
        movableTileCount = 5;
        _name = "전사";
    }
}
