
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public enum ClientState {
        // Pre-game states
        ToConnect, 
        WaitingGame, 
        // In-game states
        Playing, 
        WaitingPlayers, 
        WaitingServer, 
        Updating
    }
    ClientState currentState = ClientState.ToConnect;

    public ServerController.ServerState serverState;

    ClientCommunication clientCommunication;

    int _clientId;
    public int ClientId{
        get => _clientId;
    }

    // Start is called before the first frame update
    void Start(){
        InvokeRepeating("UpdateStati", 1f, 1f);
    }

    void OnGUI(){
        switch(currentState){
            case ClientState.ToConnect:
                GUIToConnectState();
            break;
            case ClientState.WaitingGame:
                GUIWaitingGameState();
            break;
            case ClientState.Playing:
                GUIPlayingState();
            break;
            case ClientState.WaitingPlayers:
                GUIWaitingPlayersState();
            break;
            case ClientState.WaitingServer:
                GUIWaitingServerState();
            break;
            case ClientState.Updating:
                GUIUpdatingState();
            break;
        }
        
    }

    // Update is called once per second
    void UpdateStati(){
        switch(currentState){
            case ClientState.WaitingGame:
                if(serverState == ServerController.ServerState.SetUp){
                    clientCommunication.ScheduleGetStateRequest();
                }else{
                    currentState = ClientState.Playing;
                }
            break;
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

    //////// On GUI methods
    void createMidScreenText(string text){
        GUILayout.BeginArea(new Rect(100, 10, 100, 100));
        GUILayout.TextArea(text);
        GUILayout.EndArea();
    }
    void GUIToConnectState(){
        if (GUILayout.Button("JOIN GAME")){
            SetClientIdentity();
            clientCommunication = gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
            currentState = ClientState.WaitingGame;
        }
    }

    void GUIWaitingGameState(){
        createMidScreenText("Waiting players to enter");
    }

    void GUIWaitingPlayersState(){
        createMidScreenText("Waiting players turn");
    }

    void GUIPlayingState(){
        if (GUILayout.Button("Set PutPlay")){
            clientCommunication.SchedulePutPlayRequest(_clientId, new Vector2Int(66,66), new Vector2Int(44,44), false);
            currentState = ClientState.WaitingPlayers;
        }
    }

    void GUIWaitingServerState(){
        createMidScreenText("What happened");
    }

    void GUIUpdatingState(){
        createMidScreenText("Updating ship");
    }


    //////// Update logic methods


    public void SetClientIdentity(){
        _clientId = Mathf.Abs(this.GetInstanceID() + System.DateTime.Now.Second);
    }
}
