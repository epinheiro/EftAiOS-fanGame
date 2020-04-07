using UnityEngine;

public class SetUpState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;
    UIController uiController;

    readonly string buttonText = "Begin match with {0} players";

    public SetUpState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
        this.uiController = serverController.UIController;
    }

    protected override void ExecuteLogic(){
        if(IsPossibleToBeginMatch()){
            uiController.SetConditionalButtonVisibility(true);
            this.uiController.SetConditionalButtonText(ButtonText(serverCommunication.ConnectionQuantity));
        }else{
            uiController.SetConditionalButtonVisibility(false);
            this.uiController.SetConditionalButtonText(ButtonText(serverCommunication.ConnectionQuantity));
        }
    }

    string ButtonText(int numberOfPlayers){
        return string.Format(buttonText, numberOfPlayers);
    }

    protected override void GUISetter(){
        this.uiController.SetConditionalButtonLayout(ButtonText(serverCommunication.ConnectionQuantity), IPStrings(), delegate(){ Callback(); });
    }

    public string IPStrings(){
        return string.Format("{0} or {1}", NodeCommunication.GetLocalIPAddress(), NodeCommunication.GetExternalIPAddress());
    }

    public void Callback(){
        StateEnd();
    }

    bool IsPossibleToBeginMatch(){
        return serverCommunication.ConnectionQuantity >= 2;
    }

    protected override void StateEnd(){
        serverController.PlayersPlaying = serverCommunication.ConnectionQuantity;
        serverController.PlayersEscaped = 0;
        serverController.PlayersDead = 0;

        PreparePossibleRoles();
        serverController.CreateBoardManager();
        ResetStateController();
        serverController.NextState = ServerController.ServerState.WaitingPlayers;

        this.uiController.SetPresetLayout(UIController.Layout.BoardDefault);
        this.uiController.SetProgressBarValues(serverController.TurnLimit-serverController.TurnsLeft, serverController.TurnLimit);
    }

    void PreparePossibleRoles(){
        int playersNumber = serverCommunication.ConnectionQuantity;
        int numberHalf = playersNumber/2;
        int alienModifier = playersNumber%2==0 ? 0 : 1; // There is always an even number of aliens - or 1 more

        serverController.PlayerRolesToGive.AddRedundant(ClientController.PlayerState.Alien, numberHalf + alienModifier);
        serverController.PlayerRolesToGive.AddRedundant(ClientController.PlayerState.Human, numberHalf);

        serverController.PlayerRolesToGive.Shuffle();
    }
}
