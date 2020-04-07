﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ServerController : BaseController
{
    public enum ServerState {
        // Pre-game states
        SetUp, 
        // In-game states
        WaitingPlayers, 
        Processing, 
        Updating,
        // Post-game states
        EndGame
    }

    Dictionary<ServerState, IStateController> states;

    int _turnLimit = 39;
    public int TurnLimit{
        get { return _turnLimit; }
    }
    int _turnCountdown;
    public int TurnsLeft{
        get { return _turnCountdown; }
    }
    public int DecreaseTurnNumber(){
        int nextTurn = (_turnLimit) - (--_turnCountdown);
        UIController.SetProgressBarValues(nextTurn, _turnLimit);
        return nextTurn;
    }


    ServerState _currentState = ServerState.SetUp;
    public ServerState CurrentState{
        get { return _currentState; }
    }

    ServerState nextState = ServerState.SetUp;
    public ServerState NextState{
        get { return nextState; }
        set { nextState = value; }
    }

    ServerCommunication serverCommunication;

    public int _playersPlaying;
    public int PlayersPlaying{
        set{
            _playersPlaying = value;
        }
        get{ return _playersPlaying; }
    }
    public int _playersEscaped;
    public int PlayersEscaped{
        set{
            _playersEscaped = value;
        }
        get{ return _playersEscaped; }
    }
    public int _playersDead;
    public int PlayersDead{
        set{
            _playersDead = value;
        }
        get{ return _playersDead; }
    }

    Dictionary<int, PlayerTurnData> playerTurnDict;
    public Dictionary<int, PlayerTurnData> PlayerTurnDict{
        get{ return playerTurnDict; }
    }
    public void PlayerDisconnection(int playerId){
        --PlayersPlaying;
        ++PlayersDead;
        playerTurnDict.Remove(playerId);
    }

    ExtendedList<ClientController.PlayerState> playerRolesToGive;
    public ExtendedList<ClientController.PlayerState> PlayerRolesToGive{
        get { return playerRolesToGive; }
    }

    void Awake(){
        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;
    }

    void Start(){
        InstantiateUICanvas();

        SetUp();
    }

    void SetUp(){
        playerRolesToGive = new ExtendedList<ClientController.PlayerState>();
        playerTurnDict = new Dictionary<int, PlayerTurnData>();

        states = new Dictionary<ServerState, IStateController>();
        states.Add(ServerState.SetUp, new SetUpState(this, serverCommunication));
        states.Add(ServerState.WaitingPlayers, new WaitingPlayersServerState(this, serverCommunication));
        states.Add(ServerState.Processing, new ProcessingState(this, serverCommunication));
        states.Add(ServerState.Updating, new UpdatingServerState(this, serverCommunication));
        states.Add(ServerState.EndGame, new EndGameState(this));

        _turnCountdown = _turnLimit;

        PlayersPlaying = 0;
        PlayersEscaped = 0;
        PlayersDead = 0;
    }

    public void Reset(){
        SceneManager.LoadScene("OnlyServer");
    }

    // Update is called once per frame
    void Update(){
        if (_currentState != nextState){
            _currentState = nextState;
        }

        IStateController state;
        states.TryGetValue(_currentState, out state);
        state.Execute();
    }

    public void InsertNewPlayTurnData(int playerId, Vector2Int movementTo, Vector2Int soundIn, bool attacked){
        PutPlayRequest playerData = new PutPlayRequest(playerId, movementTo.x, movementTo.y, soundIn.x, soundIn.y, attacked);
        InsertNewPlayTurnData(playerData);
    }

    public void InsertNewPlayTurnData(PutPlayRequest putPlayData){
        int playerId = putPlayData.playerId;

        PlayerTurnData turnData;
        if(playerTurnDict.TryGetValue(playerId, out turnData)){
            turnData.InputNewPutPlay(putPlayData);
        }else{
            throw new System.Exception(string.Format("SERVER - client {0} should have a play entry already", putPlayData.playerId));
        }
    }

    public void GetPlayerData(int playerId, out Vector2Int position, out ClientController.PlayerState state){
        Vector2Int finalPosition;
        ClientController.PlayerState finalState;

        if(playerRolesToGive.Count > 0){
            // Setup
            finalState = playerRolesToGive.PopValue();
            finalPosition = _boardManager.GetSpawnPointTileData(finalState).tilePosition;

            playerTurnDict.Add(
                playerId, 
                new PlayerTurnData(
                    new PutPlayRequest(playerId, finalPosition.x, finalPosition.y, finalPosition.x, finalPosition.y, false),
                    finalState
                )
            );
        }else{
            PlayerTurnData data;
            playerTurnDict.TryGetValue(playerId, out data);

            finalPosition = data.lastPlay.movementTo;
            finalState = data.role;

            switch(finalState){
                case ClientController.PlayerState.Died:
                    --PlayersPlaying;
                    ++PlayersDead;
                    playerTurnDict.Remove(playerId);
                    break;

                case ClientController.PlayerState.Escaped:
                    --PlayersPlaying;
                    ++PlayersEscaped;
                    playerTurnDict.Remove(playerId);
                    break;

                case ClientController.PlayerState.AlienOverrun:
                    playerTurnDict.Remove(playerId);
                    break;
            }
        }
        // Method outputs
        position = finalPosition;
        state = finalState;
    }

    public bool IsPossibleToProceedGame(){
        if(_turnCountdown == _turnLimit) return true;

        int countAliens = 0;
        int countHumans = 0;

        foreach(int key in PlayerTurnDict.Keys){
            PlayerTurnData data;
            PlayerTurnDict.TryGetValue(key, out data);

            switch(data.playingRole){
                case ClientController.PlayerState.Alien:
                    ++countAliens;
                    break;

                case ClientController.PlayerState.Human:
                    ++countHumans;
                    break;
            }
        }
        return !((countAliens==0 && countHumans>0) || (countAliens>0 && countHumans==0));
    }

    public void CreateBoardManager(){
        InstantiateBoardManager();
    }
}