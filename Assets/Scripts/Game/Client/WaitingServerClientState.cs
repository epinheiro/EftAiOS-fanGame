public class WaitingServerClientState : IStateController
{
    ClientController clientController; 
    UIController uiController;

    public WaitingServerClientState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(
            new ServerController.ServerState[]{ServerController.ServerState.WaitingPlayers, ServerController.ServerState.EndGame},
            ClientController.ClientState.Updating,
            delegate(){ StateEnd(); }
        );
    }
    
    protected override void GUISetter(){
        this.uiController.SetInfoText("What happened:");
    }

    protected override void StateEnd(){
        ResetStateController();
    }
}