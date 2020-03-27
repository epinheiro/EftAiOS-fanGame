using System.Collections.Generic;
using UnityEngine;

public class WaitingPlayersServerState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;
    UIController uiController;

    public WaitingPlayersServerState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
        this.uiController = serverController.UIController;
    }

    protected override void ExecuteLogic(){
        if (AllPlayersPlayed()){
            Debug.Log("SERVER - all players played");
            StateEnd();
        }
    }

    protected override void GUISetter(){
        this.uiController.SetOnlyTextLayout("Waiting player to make their move");
    }

    protected override void StateEnd(){
        serverController.BoardManagerRef.CleanLastSoundEffects();
        ResetStateController();
        serverController.NextState = ServerController.ServerState.Processing;
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
