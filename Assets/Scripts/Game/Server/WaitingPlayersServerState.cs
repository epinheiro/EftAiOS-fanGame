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
            this.uiController.SetOnlyTextInfoText("All players played");
            StateEnd();
        }else{
            this.uiController.SetOnlyTextInfoText(string.Format("{0} of {1} players waiting", WaitingPlayersNumber(), PlayingPlayersNumber()));
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

    int PlayingPlayersNumber(){
        return serverCommunication.ConnectionQuantity;
    }

    int WaitingPlayersNumber(){
        int count = 0;
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            if(data.playedThisTurn) count++;
        }
        return count;
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
