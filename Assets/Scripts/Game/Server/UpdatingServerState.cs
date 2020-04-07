using System.Collections.Generic;
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
        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);
            data.playedThisTurn = false;
        }
    }
}
