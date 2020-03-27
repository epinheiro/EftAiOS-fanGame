﻿using System.Collections.Generic;
using UnityEngine;

public class UpdatingServerState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;
    UIController uiController;

    public UpdatingServerState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
        this.uiController = serverController.UIController;
    }

    protected override void ExecuteLogic(){
        serverController.DelayedCall(UpdatingStateLogic, 1.2f); // DEBUG DELAY - TODO change
    }

    protected override void GUISetter(){
        this.uiController.SetOnlyTextLayout("Board Updating");
    }

    void UpdatingStateLogic(){
        StateEnd();
    }

    protected override void StateEnd(){
        SpawnLastNoises();
        ResetPlayerTurnControl();
        ResetStateController();
        serverController.NextState = ServerController.ServerState.WaitingPlayers;
    }

    void SpawnLastNoises(){
        List<string> noises = new List<string>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);

            if(data.playedThisTurn==false) return; // TODO - this function has been called several times this is a :poop: way to correct it poorly

            Vector2Int sound = data.lastPlay.sound;
            if(sound.x != -1){
                noises.Add(BoardManager.TranslateTileNumbersToCode(sound.x, sound.y));
            }
        }

        serverController.BoardManagerRef.LastSoundEffects(noises);
    }

    public void ResetPlayerTurnControl(){
        List<int> keys = new List<int>();

        foreach(int key in serverController.PlayerTurnDict.Keys){
            keys.Add(key);
        }

        foreach(int key in keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            data.playedThisTurn = false;
        }
    }
}
