using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : PlayerInfo
{
    private void Awake()
    {
        _playerClass = PlayerClass.WARRIOR;
        MaxHP = 120;
        curHP = MaxHP;
        Atk = 45;
        def = 15;
        Range = 1;
        movableTileCount = 5;
        _name = "헬창 전사";
    }
}
