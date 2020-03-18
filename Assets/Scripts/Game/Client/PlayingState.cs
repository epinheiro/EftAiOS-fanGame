﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayingState : IStateController
{
    ClientController clientController;

    public enum TurnSteps {
        Movement,
        PlayerWillAttack,
        Card,
        Noise,
        SendData
    }
    TurnSteps currentTurnStep = TurnSteps.Movement;

    public PlayingState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
    }
    public void ShowGUI(){
        switch(currentTurnStep){
            case TurnSteps.Movement:
                if(clientController.PlayerNullableNextPosition.HasValue){
                    currentTurnStep = TurnSteps.PlayerWillAttack;
                }

                break;

            case TurnSteps.PlayerWillAttack:
                if(clientController.CurrentPlayerState == ClientController.PlayerState.Alien){
                    if(!clientController.PlayerNullableWillAttack.HasValue){
                        GUILayout.BeginArea(new Rect(0, 0, 175, 175));
                        if(GUILayout.Button("Attack")){
                            clientController.PlayerNullableWillAttack = true;
                        }
                        if(GUILayout.Button("Quiet")){
                            clientController.PlayerNullableWillAttack = false;
                        }
                        GUILayout.EndArea();
                    }else{
                        if(clientController.PlayerNullableWillAttack.Value){
                            clientController.PlayerNullableNexSound = clientController.PlayerNullableNextPosition;
                            currentTurnStep = TurnSteps.SendData;
                        }else{
                            currentTurnStep = TurnSteps.Card;
                        }
                    }
                }else{
                    currentTurnStep = TurnSteps.Card;
                }
                break;

            case TurnSteps.Card:
                if(true){ // TODO - sort card!
                    clientController.BoardManagerRef.GlowPossibleNoises();
                    currentTurnStep = TurnSteps.Noise;
                }else{
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.Noise:
                if(clientController.PlayerNullableNexSound.HasValue){
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.SendData:
                clientController.ClientCommunication.SchedulePutPlayRequest(
                    clientController.ClientId, 
                    (Vector2Int) clientController.PlayerNullableNextPosition,
                    clientController.PlayerNullableNexSound.HasValue ? (Vector2Int) clientController.PlayerNullableNexSound : new Vector2Int(-1,-1), // TODO - check if is there better solutions than V(-1,-1)
                    clientController.PlayerNullableWillAttack.HasValue ? (bool) clientController.PlayerNullableWillAttack : false
                );
                clientController.CurrentState = ClientController.ClientState.WaitingPlayers;
                currentTurnStep = TurnSteps.Movement;
                clientController.PlayerNullableNextPosition = null;
                clientController.PlayerNullableNexSound = null;
                clientController.PlayerNullableWillAttack = null;
                break;
        }
    }
}
