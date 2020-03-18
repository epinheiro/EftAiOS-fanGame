using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingGameState : IStateController
{
    ClientController clientController; 

    public WaitingGameState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientController.ClientState.Updating, clientController.delegateBoardInstantiation);
    }
    public void ShowGUI(){
        clientController.CreateMidScreenText("Waiting players to enter");
    }
}
