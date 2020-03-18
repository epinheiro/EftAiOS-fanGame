public class UpdatingClientState : IStateController
{
    ClientController clientController; 

    public UpdatingClientState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
        switch(clientController.NextPlayerState){
            case ClientController.PlayerState.Unassigned:
                clientController.ClientCommunication.ScheduleGetResultsRequest();
            break;

            case ClientController.PlayerState.Alien:
            case ClientController.PlayerState.Human:
                clientController.CurrentPlayerState = clientController.NextPlayerState;
                clientController.NextPlayerState = ClientController.PlayerState.Unassigned;
                
                clientController.CurrentState = ClientController.ClientState.BeginTurn;
            break;
        }
    }
    public void ShowGUI(){
        switch(clientController.NextPlayerState){
            case ClientController.PlayerState.Died:
                clientController.CreateMidScreenText("You died!");
            break;
            case ClientController.PlayerState.Escaped:
                clientController.CreateMidScreenText("You won!");
            break;

            default:
                clientController.CreateMidScreenText("Updating ship");
            break;
        }
    }
}