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

    Dictionary<ClientState, IStateController> states;

    public enum PlayerState {Unassigned, Alien, Human, Died, Escaped};

    PlayerState currentPlayerState = PlayerState.Unassigned;
    public PlayerState CurrentPlayerState{
        get { return currentPlayerState; }
        set { currentPlayerState = value;}
    }
    PlayerState _nextPlayerState = PlayerState.Unassigned;
    public PlayerState NextPlayerState{
        get { return _nextPlayerState; }
        set { _nextPlayerState = value;}
    }

    /////////////////////////////////////////////
    ///// Player's turn data
    // Position
    public Vector2Int playerCurrentPosition;
    Nullable<Vector2Int> _playerNextPosition;
    public string PlayerNextPosition{
        get{ 
            if (_playerNextPosition.HasValue) return BoardManager.TranslateTileNumbersToCode(_playerNextPosition.Value.x, _playerNextPosition.Value.y);
            else return "";
        }
        set{ _playerNextPosition = BoardManager.TileCodeToVector2Int(value);}
    }
    public Nullable<Vector2Int> PlayerNullableNextPosition{
        get{ return _playerNextPosition; }
        set{ _playerNextPosition = value; }
    }

    // Sound
    public Vector2Int playerCurrentSound;
    Nullable<Vector2Int> _playerNextSound;
    public string PlayerNextSound{
        get{ 
            if (_playerNextSound.HasValue) return BoardManager.TranslateTileNumbersToCode(_playerNextSound.Value.x, _playerNextSound.Value.y);
            else return "";
        }
        set{ _playerNextSound = BoardManager.TileCodeToVector2Int(value);}
    }
    public Nullable<Vector2Int> PlayerNullableNexSound{
        get{ return _playerNextSound; }
        set{ _playerNextSound = value; }
    }

    // Attack
    Nullable<bool> _playerWillAttack;
    public Nullable<bool> PlayerNullableWillAttack{
        get{ return _playerWillAttack; }
        set{ _playerWillAttack = value; }
    }
    /////////////////////////////////////////////
    /////////////////////////////////////////////
    EventDeck deck;
    public EventDeck Deck{
        get { return deck; }
    }
    /////////////////////////////////////////////

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

    Coroutine update;

    // Start is called before the first frame update
    void Start(){
        InstantiateCanvas();

        SetUpClient();
    }

    public void SetUpClient(){
        states = new Dictionary<ClientState, IStateController>();
        states.Add(ClientState.ToConnect, new ToConnectState(this));
        states.Add(ClientState.WaitingGame, new WaitingGameState(this));
        states.Add(ClientState.BeginTurn, new BeginTurnState(this));
        states.Add(ClientState.Playing, new PlayingState(this));
        states.Add(ClientState.WaitingPlayers, new WaitingPlayersClientState(this));
        states.Add(ClientState.WaitingServer, new WaitingServerState(this));
        states.Add(ClientState.Updating, new UpdatingClientState(this));

        deck = new EventDeck();

        update = DelayedCall(UpdateStati, 1f, true);
    }

    public void ResetClient(){
        Destroy(this.GetComponent<ClientCommunication>());

        currentState = ClientController.ClientState.ToConnect;

        this.states = null;
        this.deck = null;
        StopCoroutine(update);
        this.update = null;

        Destroy(GameObject.Find("BoardManager"));

        SetUpClient();
    }

    // Update is called once per second
    void UpdateStati(){
        IStateController state;
        states.TryGetValue(currentState, out state);
        state.Execute();
    }

    /// <summary>
    /// Check if the server is on a specific state, when it gets there the client moves to another state
    /// </summary> 
    public void ChangeClientStateBaseOnServer(ServerController.ServerState expectedServerState, ClientController.ClientState nextClientState){
        if(_serverState != expectedServerState){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
        }
    }

    public void ChangeClientStateBaseOnServer(ServerController.ServerState expectedServerState, ClientController.ClientState nextClientState, BaseAction delegation){
        if(_serverState != expectedServerState){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
            delegation();
        }
    }

    public void ChangeClientStateBaseOnServer(ServerController.ServerState[] expectedServerStates, ClientController.ClientState nextClientState, BaseAction delegation){
        bool expectedServerStatesMatch = false;
        foreach(ServerController.ServerState state in expectedServerStates){
            if(_serverState == state){
                expectedServerStatesMatch = true;
                break;
            }
        }

        if(!expectedServerStatesMatch){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
            delegation();
        }
    }

    public void ScheduleGetStateRequest(){
        clientCommunication.ScheduleGetStateRequest();
    }
}
