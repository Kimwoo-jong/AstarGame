﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3 : PlayerInfo
{
    private void Awake()
    {
        _playerClass = PlayerClass.LOG;
        MaxHP = 90;
        curHP = MaxHP;
        Atk = 45;
        def = 10;
        Range = 1;
        movableTileCount = 6;
        _name = "날렵한 돚거";
    }
}
