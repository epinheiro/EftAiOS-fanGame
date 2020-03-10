
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

    public enum PlayerState {Unassigned, Alien, Human, Died, Escaped};

    PlayerState currentPlayerState = PlayerState.Unassigned;
    PlayerState _nextPlayerState = PlayerState.Unassigned;
    public PlayerState NextPlayerState{
        get { return _nextPlayerState; }
        set { _nextPlayerState = value;}
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
                WaitingGameState();                
            break;
            case ClientState.Playing:
                // Make play possible
                PlayingState();
            break;
            case ClientState.WaitingPlayers:
                // Screen of "Waiting Players"
                WaitingPlayersState();
            break;
            case ClientState.WaitingServer:
                // Screen of "What happened"
                WaitingServerState();
            break;
            case ClientState.Updating:
                // Update player position and its possible moves
                UpdatingState();
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
        createMidScreenText("What happened:");
    }

    void GUIUpdatingState(){
        createMidScreenText("Updating ship");
    }


    //////// Update logic methods
    void ToConnectState(){

    }
    void WaitingGameState(){
        ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientState.Playing);
    }
    void WaitingPlayersState(){
        ChangeClientStateBaseOnServer(ServerController.ServerState.Processing, ClientState.WaitingServer);
    }
    void PlayingState(){

    }
    void WaitingServerState(){
        ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientState.Updating);
    }
    void UpdatingState(){
        currentState = ClientState.Playing;
    }

    /// <summary>
    /// Check if the server is on a specific state, when it gets there the client moves to another state
    /// </summary> 
    void ChangeClientStateBaseOnServer(ServerController.ServerState expectedServerState, ClientController.ClientState nextClientState){
        if(serverState != expectedServerState){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
        }
    }


    public void SetClientIdentity(){
        _clientId = Mathf.Abs(this.GetInstanceID() + System.DateTime.Now.Second);
    }
}
