using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPlayersState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;

    public WaitingPlayersState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
    }

    public void ExecuteLogic(){
        if (AllPlayersPlayed()){
            Debug.Log("SERVER - all players played");
            serverController.BoardManagerRef.CleanLastSoundEffects();
            serverController.NextState = ServerController.ServerState.Processing;
        }
    }

    public void ShowGUI(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea("Waiting player to make their move");
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }

    bool AllPlayersPlayed(){
        if (serverCommunication.ConnectionQuantity != serverController.PlayerTurnDict.Count){
            return false; // The lists are inserted in the first PutPlay
        }

        foreach(KeyValuePair<int, PlayerTurnData> entry in serverController.PlayerTurnDict){
            if (!entry.Value.playedThisTurn){
                return false;
            }
        }
        return true;
    }
}
