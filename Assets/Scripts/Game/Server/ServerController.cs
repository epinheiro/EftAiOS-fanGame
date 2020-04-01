using UnityEngine;
using System.Collections.Generic;

public class ServerController : BaseController
{
    public enum ServerState {
        // Pre-game states
        SetUp, 
        // In-game states
        WaitingPlayers, 
        Processing, 
        Updating
    }

    Dictionary<ServerState, IStateController> states;

    int _turnLimit = 39;
    int _turnCountdown;
    public int TurnsLeft{
        get { return _turnCountdown; }
    }
    public int DecreaseTurnNumber(){
        return --_turnCountdown;
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

    Dictionary<int, PlayerTurnData> playerTurnDict;
    public Dictionary<int, PlayerTurnData> PlayerTurnDict{
        get{ return playerTurnDict; }
    }
    public void PlayerDisconnection(int playerId){
        playerTurnDict.Remove(playerId);
    }

    ExtendedList<ClientController.PlayerState> playerRolesToGive;
    public ExtendedList<ClientController.PlayerState> PlayerRolesToGive{
        get { return playerRolesToGive; }
    }

    void Start(){
        InstantiateCanvas();

        playerRolesToGive = new ExtendedList<ClientController.PlayerState>();
        playerTurnDict = new Dictionary<int, PlayerTurnData>();

        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;

        states = new Dictionary<ServerState, IStateController>();
        states.Add(ServerState.SetUp, new SetUpState(this, serverCommunication));
        states.Add(ServerState.WaitingPlayers, new WaitingPlayersServerState(this, serverCommunication));
        states.Add(ServerState.Processing, new ProcessingState(this, serverCommunication));
        states.Add(ServerState.Updating, new UpdatingServerState(this, serverCommunication));

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
                case ClientController.PlayerState.Escaped:
                case ClientController.PlayerState.HumanDelayed:
                    playerTurnDict.Remove(playerId);
                    break;
            }
        }
        // Method outputs
        position = finalPosition;
        state = finalState;
    }
   
    public void CreateBoardManager(){
        InstantiateBoardManager();
    }
}