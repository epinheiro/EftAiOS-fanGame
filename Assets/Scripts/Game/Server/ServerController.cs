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
    string serverIp;

    Dictionary<int, PlayerTurnData> playerTurnDict;
    public Dictionary<int, PlayerTurnData> PlayerTurnDict{
        get{ return playerTurnDict; }
    }

    ExtendedList<ClientController.PlayerState> playerRolesToGive;
    public ExtendedList<ClientController.PlayerState> PlayerRolesToGive{
        get { return playerRolesToGive; }
    }

    void Start(){

        playerRolesToGive = new ExtendedList<ClientController.PlayerState>();
        playerTurnDict = new Dictionary<int, PlayerTurnData>();

        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;
        serverIp = ServerCommunication.GetLocalIPAddress();

        states = new Dictionary<ServerState, IStateController>();
        states.Add(ServerState.SetUp, new SetUpState(this, serverCommunication));
        states.Add(ServerState.WaitingPlayers, new WaitingPlayersState(this, serverCommunication));
        states.Add(ServerState.Processing, new ProcessingState(this, serverCommunication));

    }

    void OnGUI(){
        IStateController state;
        states.TryGetValue(_currentState, out state);
        if(state != null) state.ShowGUI(); // TODO - if statement only during refactor

        switch(_currentState){
            case ServerState.Updating:
                GUIUpdatingState();
            break;
        }
    }

    // Update is called once per frame
    void Update(){
        if (_currentState != nextState){
            _currentState = nextState;
        }

        IStateController state;
        states.TryGetValue(_currentState, out state);
        if(state != null) state.ExecuteLogic(); // TODO - if statement only during refactor

        switch(_currentState){
            case ServerState.Updating:
                // Update the board
                Invoke("UpdatingState", 1.2f); // DEBUG DELAY - TODO change
                // UpdatingState();
            break;
            
        }
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

    public void ResetPlayerTurnControl(){
        List<int> keys = new List<int>();

        foreach(int key in playerTurnDict.Keys){
            keys.Add(key);
        }

        foreach(int key in keys){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(key, out data);
            data.playedThisTurn = false;
        }
    }

    public void GetPlayerData(int playerId, out Vector2Int position, out ClientController.PlayerState state){
        Vector2Int finalPosition;
        ClientController.PlayerState finalState;

        if(playerTurnDict.ContainsKey(playerId)){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(playerId, out data);

            finalPosition = data.lastPlay.movementTo;
            finalState = data.role;
        }else{ // Setup
            finalState = playerRolesToGive.PopValue();
            finalPosition = _boardManager.GetSpawnPointTileData(finalState).tilePosition;

            playerTurnDict.Add(
                playerId, 
                new PlayerTurnData(
                    new PutPlayRequest(playerId, finalPosition.x, finalPosition.y, finalPosition.x, finalPosition.y, false),
                    finalState
                )                
            );
        }

        // Method outputs
        position = finalPosition;
        state = finalState;
    }

    //////// On GUI methods
    void GUIUpdatingState(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea("Board Updating");
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }
    
    //////// Update logic methods
    public void CreateBoardManager(){
        InstantiateBoardManager(this);
    }
    void UpdatingState(){
        SpawnLastNoises();
        ResetPlayerTurnControl();
        nextState = ServerController.ServerState.WaitingPlayers;
    }

    bool AllPlayersPlayed(){
        if (serverCommunication.ConnectionQuantity != playerTurnDict.Count){
            return false; // The lists are inserted in the first PutPlay
        }

        foreach(KeyValuePair<int, PlayerTurnData> entry in playerTurnDict){
            if (!entry.Value.playedThisTurn){
                return false;
            }
        }
        return true;
    }

    void SpawnLastNoises(){
        List<string> noises = new List<string>();
        foreach(int key in playerTurnDict.Keys){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(key, out data);

            if(data.playedThisTurn==false) return; // TODO - this function has been called several times this is a :poop: way to correct it poorly

            Vector2Int sound = data.lastPlay.sound;
            if(sound.x != -1){
                noises.Add(BoardManager.TranslateTileNumbersToCode(sound.x, sound.y));
            }
        }

        BoardManagerRef.LastSoundEffects(noises);
    }
}