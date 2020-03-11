using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnData
{
    public PutPlayRequest lastPlay;
    public bool playedThisTurn;
    public ClientController.PlayerState role;

    public PlayerTurnData(PutPlayRequest lastPutPlay, ClientController.PlayerState role){
        this.lastPlay = lastPutPlay;
        this.playedThisTurn = false;
        this.role = role;
    }

    public void InputNewPutPlay(PutPlayRequest newPutPlay){
        this.lastPlay = newPutPlay;
        this.playedThisTurn = true;
    }

    public override string ToString(){
        return string.Format("Player {0} - ({1:00, 2:00}) ({3:00}{4:00}) ({5}) - Played {6} - Role {7}",
            lastPlay.playerId, lastPlay.movementTo.x, lastPlay.movementTo.y, lastPlay.sound.x, lastPlay.sound.y, lastPlay.PlayerAttacked,
            playedThisTurn, role);
    }
}
