using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public enum ClientState {ToConnect, Playing, WaitingPlayers, WaitingServer, Updating}
    ClientState currentState = ClientState.ToConnect;

    ClientCommunication clientCommunication;

    int _clientId;
    public int ClientId{
        get => _clientId;
    }

    // Start is called before the first frame update
    void Start(){
    }

    void OnGUI(){
        if (currentState == ClientState.ToConnect && GUILayout.Button("JOIN GAME")){
            SetClientIdentity();
            clientCommunication = gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
            currentState = ClientState.Updating;
        }
    }

    // Update is called once per frame
    void Update(){
        switch(currentState){
            case ClientState.Playing:
                // Make play possible
            break;
            case ClientState.WaitingPlayers:
                // Screen of "Waiting Players"
            break;
            case ClientState.WaitingServer:
                // Screen of "What happened"
            break;
            case ClientState.Updating:
                // Update player position and its possible moves
            break;
        }
        
    }

    public void SetClientIdentity(){
        _clientId = Mathf.Abs(this.GetInstanceID() + System.DateTime.Now.Second);
    }
}
