public class WaitingServerState : IStateController
{
    ClientController clientController; 
    UIController uiController;

    public WaitingServerState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientController.ClientState.Updating, delegate(){ StateEnd(); });
    }
    
    protected override void GUISetter(){
        this.uiController.SetOnlyTextLayout("What happened:");
    }

    void StateEnd(){
        ResetStateController();
    }
}