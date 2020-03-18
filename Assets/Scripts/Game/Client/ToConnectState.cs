using UnityEngine;

public class ToConnectState : IStateController
{
    ClientController clientController; 

    public ToConnectState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){}
    public void ShowGUI(){
        if (GUILayout.Button("JOIN GAME")){
            SetClientIdentity();
            clientController.ClientCommunication = clientController.gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
            clientController.CurrentState = ClientController.ClientState.WaitingGame;
        }
    }

    public void SetClientIdentity(){
        clientController.ClientId = Mathf.Abs(clientController.gameObject.GetInstanceID() + System.DateTime.Now.Second);
    }
}
