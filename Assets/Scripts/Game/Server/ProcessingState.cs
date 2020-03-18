using System.Collections.Generic;
using UnityEngine;

public class ProcessingState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;

    public ProcessingState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;

    }

    public void ExecuteLogic(){
        // Show "animation" of the turn
        serverController.DelayedCall(ProcessingStateLogic, 1.2f); // DEBUG DELAY - TODO change
        // ProcessingState();
    }

    public void ShowGUI(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea("Server processing");
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }

    public void ProcessingStateLogic(){
        ProcessResults();
        serverController.NextState = ServerController.ServerState.Updating;
    }

    void ProcessResults(){
        // Human escaped
        List<int> escapees = ProcessHumanEscapees();
        // Player kills another
        List<int> attacked = ProcessAttacks();
        // Update players stati
        UpdatePlayersStati(escapees, attacked);
        // Aliens delayed humans for 39 turns
        /// TODO turn countdown
    }

    void UpdatePlayersStati(List<int> escapess, List<int> attacked){
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
    }

    List<int> ProcessHumanEscapees(){
        List<string> escapePodCodes = serverController.BoardManagerRef.EscapePods;

        List<int> playersEscapees = new List<int>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;

            foreach(string code in escapePodCodes){
                Vector2Int escapePodePosition = BoardManager.TileCodeToVector2Int(code);

                if(data.role == ClientController.PlayerState.Human && lastPlay.movementTo == escapePodePosition){
                    playersEscapees.Add(lastPlay.playerId);
                    Debug.Log(string.Format("SERVER - player {0} escaped!", lastPlay.playerId));
                    break;
                }
            }
        }

        return playersEscapees;
    }

    List<int> ProcessAttacks(){
        // Get attacks positions
        List<Vector2Int> attackList = new List<Vector2Int>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;
            if(lastPlay.PlayerAttacked){
                attackList.Add(lastPlay.movementTo);
            }
        }

        // Check if players was in attack positions
        List<int> playersAttacked = new List<int>();
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;

            foreach(Vector2Int attackPosition in attackList){
                if(!lastPlay.PlayerAttacked && lastPlay.movementTo == attackPosition){
                    playersAttacked.Add(lastPlay.playerId);
                    Debug.Log(string.Format("SERVER - player {0} attacked!", lastPlay.playerId));
                    break;
                }
            }
        }

        return playersAttacked;
    }
}
