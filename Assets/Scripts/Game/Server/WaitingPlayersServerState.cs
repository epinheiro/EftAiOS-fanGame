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
        if(serverController.IsPossibleToProceedGame()){
            if (AllPlayersPlayed()){
                TimeLogger.Log("SERVER - all players played");
                this.uiController.SetInfoText("All passagers moved");
                this.uiController.SetPlayersStatus(0, serverController.State.PlayersAlive, serverController.State.PlayersDead, serverController.State.PlayersEscaped);
                StateEnd(ServerController.ServerState.Processing);
            }else{
                this.uiController.SetInfoText("Passengers moving");
                this.uiController.SetPlayersStatus(PlayingPlayersNumber()-WaitingPlayersNumber(), WaitingPlayersNumber(), serverController.State.PlayersDead, serverController.State.PlayersEscaped);
            }
        }else{
            StateEnd(ServerController.ServerState.EndGame);
        }
    }

    protected override void GUISetter(){
        this.uiController.SetPresetLayout(UIController.Layout.BoardDefault);
        this.uiController.SetPlayersStatus(serverController.State.PlayersAlive, 0, serverController.State.PlayersDead, serverController.State.PlayersEscaped);
    }

    void StateEnd(ServerController.ServerState nextState){
        StateEnd();
        serverController.NextState = nextState;
    }

    protected override void StateEnd(){
        serverController.BoardManagerRef.CleanLastSoundEffects();
        ResetStateController();
    }

    int PlayingPlayersNumber(){
        return serverController.PlayerTurnDict.Count;
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
