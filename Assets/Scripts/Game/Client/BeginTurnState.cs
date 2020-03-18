using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginTurnState : IStateController
{
    ClientController clientController; 

    public BeginTurnState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
        string currentTileCode = BoardManager.TranslateTileNumbersToCode(clientController.playerCurrentPosition.x, clientController.playerCurrentPosition.y);
        int movement = 0;
        switch(clientController.CurrentPlayerState){
            case ClientController.PlayerState.Alien:
                movement = 2;
                break;
            case ClientController.PlayerState.Human:
                movement = 1;
                break;
        }
        clientController.BoardManagerRef.GlowPossibleMovements(currentTileCode, movement);
        clientController.CurrentState = ClientController.ClientState.Playing;
    }
    public void ShowGUI(){
    }
}