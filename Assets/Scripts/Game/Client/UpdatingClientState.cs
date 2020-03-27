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
        Debug.Log("UpdatingClientState");
        switch(clientController.NextPlayerState){
            case ClientController.PlayerState.Unassigned:
                clientController.ClientCommunication.ScheduleGetResultsRequest();
                break;

            case ClientController.PlayerState.Alien:
            case ClientController.PlayerState.Human:
                StateEnd();
                break;

            case ClientController.PlayerState.Died:
                this.uiController.SetOnlyTextInfoText("You died!");
                clientController.ResetClient();
                break;

            case ClientController.PlayerState.Escaped:
                this.uiController.SetOnlyTextInfoText("You won!");
                clientController.ResetClient();
                break;
        }
    }
    protected override void GUISetter(){
        this.uiController.SetOnlyTextLayout("Updating ship");
    }

    protected override void StateEnd(){
        clientController.CurrentPlayerState = clientController.NextPlayerState;
        clientController.NextPlayerState = ClientController.PlayerState.Unassigned;
        
        clientController.CurrentState = ClientController.ClientState.BeginTurn;

        ResetStateController();
    }
}