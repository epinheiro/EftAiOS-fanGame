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

    void StateEnd(){
        clientController.InstantiateBoardManager();
        ResetStateController();
    }
}
