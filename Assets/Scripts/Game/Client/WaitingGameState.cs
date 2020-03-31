public class WaitingGameState : IStateController
{
    ClientController clientController;

    UIController uiController;

    public WaitingGameState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientController.ClientState.Updating, delegate(){ StateEnd(); });
    }
    protected override void GUISetter(){
        this.uiController.SetOnlyTextLayout("Waiting players to enter");
    }

    protected override void StateEnd(){
        clientController.InstantiateBoardManager();
        TimeLogger.Log("CLIENT {0} - game start", clientController.ClientId);
        ResetStateController();
    }
}
