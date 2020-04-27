public class WaitingPlayersClientState : IStateController
{
    ClientController clientController;
    UIController uiController;

    public WaitingPlayersClientState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(
            new ServerController.ServerState[]{ServerController.ServerState.Processing, ServerController.ServerState.Updating}, 
            ClientController.ClientState.WaitingServer, 
            delegate(){ StateEnd(); }
        );
    }
    protected override void GUISetter(){
        this.uiController.SetInfoText("Waiting other passengers");
    }

    protected override void StateEnd(){
        ResetStateController();
    }
}
