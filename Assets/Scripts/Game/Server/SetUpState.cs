using UnityEngine;

public class SetUpState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;
    UIController uiController;

    public SetUpState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
        this.uiController = serverController.UIController;
    }

    protected override void ExecuteLogic(){
        uiController.SetConditionalButtonVisibility(IsPossibleToBeginMatch());
    }

    protected override void GUISetter(){
        this.uiController.SetConditionalButtonLayout("Begin match", IPStrings(), delegate(){ Callback(); });
    }

    public string IPStrings(){
        return string.Format("Connect to {0} or {1}", NodeCommunication.GetLocalIPAddress(), NodeCommunication.GetExternalIPAddress());
    }

    public void Callback(){
        StateEnd();
    }

    bool IsPossibleToBeginMatch(){
        return serverCommunication.ConnectionQuantity > 0; // TODO  -DEBUG value - Correct value is 1 
    }

    void StateEnd(){
        PreparePossibleRoles();
        serverController.CreateBoardManager();
        ResetStateController();
        serverController.NextState = ServerController.ServerState.WaitingPlayers;
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
