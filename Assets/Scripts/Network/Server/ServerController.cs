using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{
    public enum ServerState {SetUp, WaitingPlayers, Processing, Updating}

    ServerState currentState = ServerState.Updating;

    ServerCommunication serverCommunication;


    void Start(){
        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;
    }

    // Update is called once per frame
    void Update(){
        switch(currentState){
            case ServerState.WaitingPlayers:
                // Keep last play on screen
            break;
            case ServerState.Processing:
                // Show "animation" of the turn
            break;
            case ServerState.Updating:
                // Update the board
            break;
            
        }
    }
}
