using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    ClientCommunication clientCommunication;

    ClientCommunication.ClientState currentState;

    int clientId;

    // Start is called before the first frame update
    void Start(){
        clientCommunication = gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
    }

    void onGui(){
        if (GUILayout.Button("Start game")){
            
            
        }
    }

    // Update is called once per frame
    void Update(){
        currentState = clientCommunication.GetState();

        switch(currentState){
            case ClientCommunication.ClientState.Playing:
                // Make play possible
            break;
            case ClientCommunication.ClientState.WaitingPlayers:
                // Screen of "Waiting Players"
            break;
            case ClientCommunication.ClientState.WaitingServer:
                // Screen of "What happened"
            break;
            case ClientCommunication.ClientState.Updating:
                // Update player position and its possible moves
            break;
        }
        
    }
}
