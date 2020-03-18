
using UnityEngine;
using System.Collections.Generic;
using System;

public class ClientController : BaseController
{
    public enum ClientState {
        // Pre-game states
        ToConnect, 
        WaitingGame, 
        // In-game states
        BeginTurn, 
        Playing,
        WaitingPlayers, 
        WaitingServer, 
        Updating
    }

    public enum TurnSteps {
        Movement,
        PlayerWillAttack,
        Card,
        Noise,
        SendData
    }

    TurnSteps currentTurnStep = TurnSteps.Movement;

    Dictionary<ClientState, IStateController> states;

    public enum PlayerState {Unassigned, Alien, Human, Died, Escaped};

    PlayerState currentPlayerState = PlayerState.Unassigned;
    PlayerState _nextPlayerState = PlayerState.Unassigned;
    public PlayerState NextPlayerState{
        get { return _nextPlayerState; }
        set { _nextPlayerState = value;}
    }
    public Vector2Int playerCurrentPosition;
    Nullable<Vector2Int> _playerNextPosition;
    public string PlayerNextPosition{
        get{ 
            if (_playerNextPosition.HasValue) return BoardManager.TranslateTileNumbersToCode(_playerNextPosition.Value.x, _playerNextPosition.Value.y);
            else return "";
        }
        set{ _playerNextPosition = BoardManager.TileCodeToVector2Int(value);}
    }

    public Vector2Int playerCurrentSound;
    Nullable<Vector2Int> _playerNextSound;
    public string PlayerNextSound{
        get{ 
            if (_playerNextSound.HasValue) return BoardManager.TranslateTileNumbersToCode(_playerNextSound.Value.x, _playerNextSound.Value.y);
            else return "";
        }
        set{ _playerNextSound = BoardManager.TileCodeToVector2Int(value);}
    }

    Nullable<bool> _playerWillAttack;

    ClientState currentState = ClientState.ToConnect;
    public ClientState CurrentState{
        get { return currentState; }
        set { currentState = value; }
    }

    ServerController.ServerState _serverState;
    public ServerController.ServerState ServerState{
        get { return _serverState; }
        set { _serverState = value; }
    }

    ClientCommunication clientCommunication;
    public ClientCommunication ClientCommunication{
        get { return clientCommunication; }
        set { clientCommunication = value; }
    }

    int _clientId;
    public int ClientId{
        get => _clientId;
        set { _clientId = value; }
    }

    delegate void ClientControllerDelegateAction(ClientController client);
    ClientControllerDelegateAction delegateBoardInstantiation = InstantiateBoardManager;

    static protected void InvokeCleanHighlights(BaseController controller){
        controller.BoardManagerRef.CleanGlowTiles();
    }

    // Start is called before the first frame update
    void Start(){
        states = new Dictionary<ClientState, IStateController>();
        states.Add(ClientState.ToConnect, new ToConnectState(this));

        DelayedCall(UpdateStati, 1f, true);
    }

    void OnGUI(){
        IStateController state;
        states.TryGetValue(currentState, out state);
        if(state!=null) state.ShowGUI(); // TODO - if statement used during refactor

        switch(currentState){
            case ClientState.WaitingGame:
                GUIWaitingGameState();
            break;
            case ClientState.Playing:
                GUIPlayingTurnState();
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
        IStateController state;
        states.TryGetValue(currentState, out state);
        if(state!=null) state.ExecuteLogic(); // TODO - if statement used during refactor

        switch(currentState){
            case ClientState.WaitingGame:
                WaitingGameState();                
            break;
            case ClientState.BeginTurn:
                // Make play possible
                BeginTurnState();
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

    void GUIWaitingGameState(){
        createMidScreenText("Waiting players to enter");
    }

    void GUIWaitingPlayersState(){
        createMidScreenText("Waiting players turn");
    }

    void GUIPlayingTurnState(){
        switch(currentTurnStep){
            case TurnSteps.Movement:
                if(_playerNextPosition.HasValue){
                    currentTurnStep = TurnSteps.PlayerWillAttack;
                }

                break;

            case TurnSteps.PlayerWillAttack:
                if(currentPlayerState == PlayerState.Alien){
                    if(!_playerWillAttack.HasValue){
                        GUILayout.BeginArea(new Rect(0, 0, 175, 175));
                        if(GUILayout.Button("Attack")){
                            _playerWillAttack = true;
                        }
                        if(GUILayout.Button("Quiet")){
                            _playerWillAttack = false;
                        }
                        GUILayout.EndArea();
                    }else{
                        if(_playerWillAttack.Value){
                            _playerNextSound = _playerNextPosition;
                            currentTurnStep = TurnSteps.SendData;
                        }else{
                            currentTurnStep = TurnSteps.Card;
                        }
                    }
                }else{
                    currentTurnStep = TurnSteps.Card;
                }
                break;

            case TurnSteps.Card:
                if(true){ // TODO - sort card!
                    BoardManagerRef.GlowPossibleNoises();
                    currentTurnStep = TurnSteps.Noise;
                }else{
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.Noise:
                if(_playerNextSound.HasValue){
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.SendData:
                clientCommunication.SchedulePutPlayRequest(
                    _clientId, 
                    (Vector2Int) _playerNextPosition,
                    _playerNextSound.HasValue ? (Vector2Int) _playerNextSound : new Vector2Int(-1,-1), // TODO - check if is there better solutions than V(-1,-1)
                    _playerWillAttack.HasValue ? (bool) _playerWillAttack : false
                );
                currentState = ClientState.WaitingPlayers;
                currentTurnStep = TurnSteps.Movement;
                _playerNextPosition = null;
                _playerNextSound = null;
                _playerWillAttack = null;
                break;
        }
    }

    void GUIWaitingServerState(){
        createMidScreenText("What happened:");
    }

    void GUIUpdatingState(){
        switch(NextPlayerState){
            case PlayerState.Died:
                createMidScreenText("You died!");
            break;
            case PlayerState.Escaped:
                createMidScreenText("You won!");
            break;

            default:
                createMidScreenText("Updating ship");
            break;
        }
    }


    //////// Update logic methods
    void ToConnectState(){

    }
    void WaitingGameState(){
        ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientState.Updating, delegateBoardInstantiation);
    }
    void WaitingPlayersState(){
        ChangeClientStateBaseOnServer(ServerController.ServerState.Processing, ClientState.WaitingServer, InvokeCleanHighlights);
    }
    void BeginTurnState(){
        string currentTileCode = BoardManager.TranslateTileNumbersToCode(playerCurrentPosition.x, playerCurrentPosition.y);
        int movement = 0;
        switch(currentPlayerState){
            case PlayerState.Alien:
                movement = 2;
                break;
            case PlayerState.Human:
                movement = 1;
                break;
        }
        BoardManagerRef.GlowPossibleMovements(currentTileCode, movement);
        currentState = ClientState.Playing;
    }
    void WaitingServerState(){
        ChangeClientStateBaseOnServer(ServerController.ServerState.WaitingPlayers, ClientState.Updating);
    }
    void UpdatingState(){
        switch(NextPlayerState){
            case PlayerState.Unassigned:
                clientCommunication.ScheduleGetResultsRequest();
            break;

            case PlayerState.Alien:
            case PlayerState.Human:
                currentPlayerState = NextPlayerState;
                NextPlayerState = PlayerState.Unassigned;
                
                currentState = ClientState.BeginTurn;
            break;
        }
    }

    /// <summary>
    /// Check if the server is on a specific state, when it gets there the client moves to another state
    /// </summary> 
    void ChangeClientStateBaseOnServer(ServerController.ServerState expectedServerState, ClientController.ClientState nextClientState){
        if(_serverState != expectedServerState){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
        }
    }

    void ChangeClientStateBaseOnServer(ServerController.ServerState expectedServerState, ClientController.ClientState nextClientState, ClientControllerDelegateAction delegation){
        if(_serverState != expectedServerState){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
            delegation(this);
        }
    }
}
