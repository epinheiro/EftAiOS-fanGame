using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPlayersClientState : IStateController
{
    ClientController clientController; 

    public WaitingPlayersClientState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.Processing, ClientController.ClientState.WaitingServer, InvokeCleanHighlights);
    }
    public void ShowGUI(){
        clientController.CreateMidScreenText("Waiting players turn");
    }

    static protected void InvokeCleanHighlights(BaseController controller){
        controller.BoardManagerRef.CleanGlowTiles();
    }
}
