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
        this.uiController.SetInfoText("Board Updating");
    }

    void UpdatingStateLogic(){
        StateEnd();
    }

    protected override void StateEnd(){
        SpawnLastNoises();
        ResetTurnControlVariables();
        ResetStateController();
        serverController.NextState = ServerController.ServerState.WaitingPlayers;
    }

    void SpawnLastNoises(){
        List<NoiseInfo> noises = new List<NoiseInfo>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);

            if(data.playedThisTurn==false) return; // TODO - this function has been called several times this is a :poop: way to correct it poorly

            Vector2Int sound = data.lastPlay.sound;
            if(sound.x != -1){
                string tileCode = BoardManager.TranslateTileNumbersToCode(sound.x, sound.y);
                noises.Add(new NoiseInfo(tileCode, data.lastPlay.PlayerAttacked));
            }
        }

        serverController.BoardManagerRef.LastSoundEffects(noises);
    }

    public void ResetTurnControlVariables(){
        int playersPlaying = serverController.playersPlaying + serverController.playersEscaped + serverController.playersDied;
        int playersEscaped = 0;
        int playersDied = 0;

        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            data.playedThisTurn = false;

            switch(data.role){
                case ClientController.PlayerState.Died:
                    --playersPlaying;
                    ++playersDied;
                    break;

                case ClientController.PlayerState.Escaped:
                    --playersPlaying;
                    ++playersEscaped;
                    break;

                // case ClientController.PlayerState.Alien:
                // case ClientController.PlayerState.Human:
                // case ClientController.PlayerState.AlienOverrun:

                case ClientController.PlayerState.Unassigned:
                    throw new System.Exception("In this step was not suppose to be Unassigned player roles");
            }
        }

        serverController.playersPlaying = playersPlaying;
        serverController.playersEscaped = playersEscaped;
        serverController.playersDied = playersDied;
    }
}
