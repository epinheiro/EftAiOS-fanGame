﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public GameState(){
        SetGameState(0);
    }

    //////////////////////////////////////////
    ///////////// Players control ////////////
    //////////////////////////////////////////
    public int _playersAlive;
    public int _playersEscaped;
    public int _playersDead;

    public int PlayersAlive{
        get{ return _playersAlive; }
    }
    public int PlayersEscaped{
        get{ return _playersEscaped; }
    }
    public int PlayersDead{
        get{ return _playersDead; }
    }

    //// Public API
    public void SetGameState(int playersAlive, int playersDead=0, int playersEscaped=0){
        _playersAlive = playersAlive;
        _playersDead = playersDead;
        _playersEscaped = playersEscaped;
    }

    public void IncreaseDead(){
        --_playersAlive;
        ++_playersDead;
    }

    public void IncreaseEscapees(){
        --_playersAlive;
        ++_playersEscaped;
    }
}
