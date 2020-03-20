using UnityEngine;

public class SetUpState : IStateController
{
    ServerController serverController;
    ServerCommunication serverCommunication;

    public SetUpState(ServerController serverController, ServerCommunication serverCommunication){
        this.serverController = serverController;
        this.serverCommunication = serverCommunication;
    }

    public void ExecuteLogic(){

    }

    public void ShowGUI(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea(string.Format("Connect to LAN: {0}", NodeCommunication.GetLocalIPAddress()));
        GUILayout.TextArea(string.Format("Connect to IP:  {0}", NodeCommunication.GetExternalIPAddress()));
        if(IsPossibleToBeginMatch()){
            if (GUILayout.Button("Start game")){
                SetUpStateEnd();
            }
        }

        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }

    bool IsPossibleToBeginMatch(){
        return serverCommunication.ConnectionQuantity > 0; // TODO  -DEBUG value - Correct value is 1 
    }

    void SetUpStateEnd(){
        PreparePossibleRoles();
        serverController.CreateBoardManager();
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
