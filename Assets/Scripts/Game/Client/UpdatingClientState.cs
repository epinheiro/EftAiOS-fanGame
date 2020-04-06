using UnityEngine;

public class UpdatingClientState : IStateController
{
    ClientController clientController;
    UIController uiController;

    public UpdatingClientState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        switch(clientController.NextPlayerState){
            case ClientController.PlayerState.Unassigned:
                clientController.ClientCommunication.ScheduleGetResultsRequest();
                break;

            case ClientController.PlayerState.Alien:
            case ClientController.PlayerState.Human:
                StateEnd();
                break;

            case ClientController.PlayerState.Died:
                this.uiController.SetInfoText("You died!");
                clientController.ResetClient();
                break;

            case ClientController.PlayerState.Escaped:
                this.uiController.SetInfoText("You escaped the ship!");
                clientController.ResetClient();
                break;

            case ClientController.PlayerState.AlienOverrun:
                this.uiController.SetInfoText("You surpassed the humans!");
                clientController.ResetClient();
                break;
        }
    }
    protected override void GUISetter(){
        this.uiController.SetInfoText("Updating ship");
    }

    protected override void StateEnd(){
        clientController.CurrentPlayerState = clientController.NextPlayerState;
        clientController.NextPlayerState = ClientController.PlayerState.Unassigned;
        
        clientController.CurrentState = ClientController.ClientState.BeginTurn;

        ResetStateController();
    }
}