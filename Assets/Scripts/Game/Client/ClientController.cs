﻿
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
    public PlayerState CurrentPlayerState{
        get { return currentPlayerState; }
        set { currentPlayerState = value;}
    }
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

    public delegate void ClientControllerDelegateAction(ClientController client);
    public ClientControllerDelegateAction delegateBoardInstantiation = InstantiateBoardManager;

    // Start is called before the first frame update
    void Start(){
        states = new Dictionary<ClientState, IStateController>();
        states.Add(ClientState.ToConnect, new ToConnectState(this));
        states.Add(ClientState.WaitingGame, new WaitingGameState(this));
        states.Add(ClientState.BeginTurn, new BeginTurnState(this));

        states.Add(ClientState.WaitingPlayers, new WaitingPlayersClientState(this));
        states.Add(ClientState.WaitingServer, new WaitingServerState(this));
        states.Add(ClientState.Updating, new UpdatingClientState(this));

        DelayedCall(UpdateStati, 1f, true);
    }

    void OnGUI(){
        IStateController state;
        states.TryGetValue(currentState, out state);
        if(state!=null) state.ShowGUI(); // TODO - if statement used during refactor

        switch(currentState){
            case ClientState.Playing:
                GUIPlayingTurnState();
            break;
        }
        
    }

    // Update is called once per second
    void UpdateStati(){
        IStateController state;
        states.TryGetValue(currentState, out state);
        if(state!=null) state.ExecuteLogic(); // TODO - if statement used during refactor
    }

    //////// On GUI methods
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

    public void ChangeClientStateBaseOnServer(ServerController.ServerState expectedServerState, ClientController.ClientState nextClientState, ClientControllerDelegateAction delegation){
        if(_serverState != expectedServerState){
            clientCommunication.ScheduleGetStateRequest();
        }else{
            currentState = nextClientState;
            delegation(this);
        }
    }
}
