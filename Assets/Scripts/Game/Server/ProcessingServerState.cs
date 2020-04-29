using System.Collections.Generic;
using UnityEngine;

public class ProcessingServerState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;
    UIController uiController;

    bool isProcessing;

    public ProcessingServerState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
        this.uiController = serverController.UIController;
        isProcessing = false;
    }

    protected override void ExecuteLogic(){
        // Show "animation" of the turn
        if(!isProcessing){
            isProcessing = true;
            serverController.DelayedCall(ProcessingStateLogic, 1.2f); // DEBUG DELAY - TODO change
        }
        // ProcessingState();
    }

    protected override void GUISetter(){
        this.uiController.SetInfoText("Processing");
    }

    public void ProcessingStateLogic(){
        ProcessResults();
        StateEnd();
    }

    protected override void StateEnd(){
        ResetStateController();
        isProcessing = false;
        serverController.NextState = ServerController.ServerState.Updating;
    }

    void ProcessResults(){
        /// Turn countdown
        serverController.DecreaseTurnNumber();

        // Human escaped
        List<int> escapees = ProcessHumanEscapees();
        // Player kills another
        List<int> attacked = ProcessAttacks();

        // If Aliens delayed humans for 39 turns
        bool isFinalTurn = serverController.TurnsLeft <= 0;

        // If with the remaining roles is possible to end the game
        bool isPossibleToProceed = true;
        if(!isFinalTurn){
            isPossibleToProceed = serverController.IsPossibleToProceedGame();
        }

        // Update players stati
        UpdatePlayersStati(escapees, attacked, isFinalTurn, isPossibleToProceed);
    }

    void UpdatePlayersStati(List<int> escapess, List<int> attacked, bool isFinalTurn, bool isPossibleToProceed){
        foreach(int code in escapess){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(code, out data);
            data.role = ClientController.PlayerState.Escaped;
        }

        foreach(int code in attacked){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(code, out data);
            data.role = ClientController.PlayerState.Died;
        }

        if(isFinalTurn){
            foreach(int key in serverController.PlayerTurnDict.Keys){
                PlayerTurnData data;
                serverController.PlayerTurnDict.TryGetValue(key, out data);

                switch(data.role){
                    case ClientController.PlayerState.Human:
                        data.role = ClientController.PlayerState.Died;
                        break;
                    case ClientController.PlayerState.Alien:
                        data.role = ClientController.PlayerState.AlienOverrun;
                        break;
                }
            }
        }else{
            if(!isPossibleToProceed){
                // In possible future ADVANCED game will need another methods
                foreach(int key in serverController.PlayerTurnDict.Keys){
                    PlayerTurnData data;
                    serverController.PlayerTurnDict.TryGetValue(key, out data);

                    switch(data.role){
                        case ClientController.PlayerState.Human:
                            data.role = ClientController.PlayerState.Escaped;
                            break;
                        case ClientController.PlayerState.Alien:
                            data.role = ClientController.PlayerState.AlienOverrun;
                            break;
                    }
                }
            }
        }
    }

    List<int> ProcessHumanEscapees(){
        List<int> playersEscapees = new List<int>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;

            if(data.role == ClientController.PlayerState.Human && serverController.BoardManagerRef.GetTileType(BoardManager.TranslateTilePositionToCode(lastPlay.movementTo)) == BoardManager.PossibleTypes.EscapePod){
                playersEscapees.Add(lastPlay.playerId);
                TimeLogger.Log("SERVER - player {0} escaped!", lastPlay.playerId);
                serverController.State.IncreaseEscapees();
                break;
            }
        }

        return playersEscapees;
    }

    List<int> ProcessAttacks(){
        List<Color> attackingColors = new List<Color>();

        // Get attacks positions
        List<Vector2Int> attackList = new List<Vector2Int>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;
            if(lastPlay.PlayerAttacked){
                attackList.Add(lastPlay.movementTo);
                attackingColors.Add(data.GetUIColorMaterial().color);
            }
        }

        uiController.SetAlienFeedback(attackingColors);

        // Check if players was in attack positions
        List<int> playersAttacked = new List<int>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;

            foreach(Vector2Int attackPosition in attackList){
                if(!lastPlay.PlayerAttacked && lastPlay.movementTo == attackPosition){
                    playersAttacked.Add(lastPlay.playerId);
                    serverController.State.IncreaseDead();
                    TimeLogger.Log("SERVER - player {0} attacked!", lastPlay.playerId);
                    break;
                }
            }
        }

        return playersAttacked;
    }
}
