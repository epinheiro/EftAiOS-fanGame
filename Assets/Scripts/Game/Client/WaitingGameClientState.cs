public class WaitingGameClientState : IStateController
{
    ClientController clientController;

    UIController uiController;

    public WaitingGameClientState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientController.ClientState.Updating, delegate(){ StateEnd(); });
    }
    protected override void GUISetter(){
        this.uiController.SetPresetLayout(UIController.Layout.ClientDefault);
        this.uiController.SetInfoText("Waiting passengers to awake");
    }

    protected override void StateEnd(){
        clientController.InstantiateBoardManager();

        uiController.DeactiveBackButton();

        TimeLogger.Log("CLIENT {0} - game start", clientController.ClientId);
        ResetStateController();
    }
}
