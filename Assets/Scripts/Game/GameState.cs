using System.Collections;
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
    int _playersAlive;
    int _playersEscaped;
    int _playersDead;

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

    public void IncreaseDead(int newDeads = 1){
        _playersAlive -= newDeads;
        _playersDead  += newDeads;
    }

    public void IncreaseEscapees(int newEscapees = 1){
        _playersAlive -= newEscapees;
        _playersDead  += newEscapees;
    }
}
