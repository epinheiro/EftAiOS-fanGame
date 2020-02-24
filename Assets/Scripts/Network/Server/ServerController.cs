using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{
    ServerCommunication serverCommunication;

    ServerCommunication.ServerState currentState;

    void Start(){
        serverCommunication = this.GetComponent<ServerCommunication>();
    }

    // Update is called once per frame
    void Update(){
        currentState = serverCommunication.GetState();

        switch(currentState){
            case ServerCommunication.ServerState.WaitingPlayers:
                // Keep last play on screen
            break;
            case ServerCommunication.ServerState.Processing:
                // Show "animation" of the turn
            break;
            case ServerCommunication.ServerState.Updating:
                // Update the board
            break;
            
        }
    }
}
