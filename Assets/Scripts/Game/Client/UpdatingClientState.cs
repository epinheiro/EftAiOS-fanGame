﻿using UnityEngine;

public class UpdatingClientState : IStateController
{
    bool receivedResponse = false;

    ClientController clientController;
    UIController uiController;

    public UpdatingClientState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        if(!receivedResponse){
            switch(clientController.NextPlayerState){
                case ClientController.PlayerState.Unassigned:
                    clientController.ClientCommunication.ScheduleGetResultsRequest();
                    break;

                case ClientController.PlayerState.Alien:
                case ClientController.PlayerState.Human:
                    receivedResponse = true;
                    StateEnd();
                    break;

                case ClientController.PlayerState.Died:
                    receivedResponse = true;
                    this.uiController.SetInfoText("You died!");
                    this.clientController.Audio.PlayerDiedEffect();
                    clientController.DelayedCall(clientController.SoftResetClient, 3f);
                    break;

                case ClientController.PlayerState.Escaped:
                    receivedResponse = true;
                    this.uiController.SetInfoText("You escaped the ship!");
                    this.clientController.Audio.PlayerEscapedEffect();
                    clientController.DelayedCall(clientController.SoftResetClient, 3f);
                    break;

                case ClientController.PlayerState.AlienOverrun:
                    receivedResponse = true;
                    this.uiController.SetInfoText("You surpassed the humans!");
                    this.clientController.Audio.AlienOverrunEffect();
                    clientController.DelayedCall(clientController.SoftResetClient, 3f);
                    break;
            }
        }
    }
    protected override void GUISetter(){
        this.uiController.SetInfoText("Updating ship");
    }

    protected override void StateEnd(){
        receivedResponse = false;

        clientController.CurrentPlayerState = clientController.NextPlayerState;
        clientController.NextPlayerState = ClientController.PlayerState.Unassigned;
        
        clientController.CurrentState = ClientController.ClientState.BeginTurn;

        ResetStateController();
    }
}