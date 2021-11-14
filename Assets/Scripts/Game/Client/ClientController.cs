using UnityEngine;
using UnityEngine.SceneManagement;
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

    string _lastSucessfulIP = "";
    public string LastSucessfulIp{
        get { return _lastSucessfulIP; }
        set { _lastSucessfulIP = value; }
    }


    Dictionary<ClientState, IStateController> states;

    public enum PlayerState {Unassigned, Alien, Human, Died, Escaped, AlienOverrun};

    PlayerTurnData.UIColors _playerColor;
    public PlayerTurnData.UIColors PlayerColor{
        get { return _playerColor; }
        set { _playerColor = value; }
    }

    PlayerState currentPlayerState = PlayerState.Unassigned;
    public PlayerState CurrentPlayerState{
        get { return currentPlayerState; }
        set {
            if(currentPlayerState == PlayerState.Unassigned){
                UIController.SetPlayerColor(_playerColor);
                UIController.SetPlayerRole(value);
                UIController.ShowRolePopup();
            }
            currentPlayerState = value;
        }
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
        protected set { clientCommunication = value; }
    }

    int _clientId;
    public int ClientId{
        get => _clientId;
        set { _clientId = value; }
    }

    Coroutine update;

    void Awake(){
        try{
            FileAsset.GameData data = FileAsset.LoadGameData();
            _lastSucessfulIP = data.lastIP;

        } catch (System.Exception e){
            TimeLogger.Log("No last IP found: {0}", e.Message);
        }

        SetupApplication();
    }

    void Start(){
        GetUICanvasReference();

        SetUpClient();

        InstantiateAudioController();
    }

    public void SetUpClient(){
        states = new Dictionary<ClientState, IStateController>();
        states.Add(ClientState.ToConnect, new ToConnectClientState(this, _lastSucessfulIP));
        states.Add(ClientState.WaitingGame, new WaitingGameClientState(this));
        states.Add(ClientState.BeginTurn, new BeginTurnClientState(this));
        states.Add(ClientState.Playing, new PlayingClientState(this));
        states.Add(ClientState.WaitingPlayers, new WaitingPlayersClientState(this));
        states.Add(ClientState.WaitingServer, new WaitingServerClientState(this));
        states.Add(ClientState.Updating, new UpdatingClientState(this));

        deck = new EventDeck();

        update = DelayedCall(UpdateStati, 1f, true);
    }

    public void StartClientCommunication()
    {
        ClientCommunication = this.gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
    }

    public void Reset(){
        LoadScene("OnlyClient");
    }

    public void SoftResetClient(){
        Destroy(this.GetComponent<ClientCommunication>());

        currentState = ClientController.ClientState.ToConnect;

        // Erasing client GAME data
        currentPlayerState = PlayerState.Unassigned;
        _nextPlayerState = PlayerState.Unassigned;
        _clientId = -1;
        playerCurrentPosition = new Vector2Int(-1,-1);
        _playerNextPosition = null;
        playerCurrentSound = new Vector2Int(-1,-1);
        _playerNextSound = null;
        _playerWillAttack = null;

        this.states = null;
        this.deck = null;
        StopCoroutine(update);
        this.update = null;

        Destroy(GameObject.Find("BoardManager"));

        SetUpClient();
    }

    public void ReloadClient(){
        LoadScene("OnlyClient");
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
        foreach(ServerController.ServerState state in expectedServerStates){
            if(_serverState == state){

                TimeLogger.Log("CLIENT {0} - server changed to {1}", ClientId, state);
                currentState = nextClientState;

                delegation();

                return;
            }
        }

        clientCommunication.ScheduleGetStateRequest();
    }

    public void ScheduleGetStateRequest(){
        clientCommunication.ScheduleGetStateRequest();
    }
}
