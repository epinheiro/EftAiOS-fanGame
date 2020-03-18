using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatingServerState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;

    public UpdatingServerState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;

    }

    public void ExecuteLogic(){
        serverController.DelayedCall(UpdatingStateLogic, 1.2f); // DEBUG DELAY - TODO change
    }

    public void ShowGUI(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea("Board Updating");
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }

    void UpdatingStateLogic(){
        SpawnLastNoises();
        ResetPlayerTurnControl();
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
