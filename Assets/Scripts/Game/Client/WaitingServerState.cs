using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingServerState : IStateController
{
    ClientController clientController; 

    public WaitingServerState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientController.ClientState.Updating);
    }
    
    public void ShowGUI(){
        clientController.CreateMidScreenText("What happened:");
    }
}