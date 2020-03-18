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

    }

    void OnGUI(){
        IStateController state;
        states.TryGetValue(_currentState, out state);
        if(state != null) state.ShowGUI(); // TODO - if statement only during refactor

        switch(_currentState){
            case ServerState.WaitingPlayers:
                GUIWaitingPlayersState();
            break;

            case ServerState.Processing:
                GUIProcessingState();
            break;

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
            case ServerState.WaitingPlayers:
                // Keep last play on screen
                WaitingPlayersState();
            break;
            case ServerState.Processing:
                // Show "animation" of the turn
                Invoke("ProcessingState", 1.2f); // DEBUG DELAY - TODO change
                // ProcessingState();
            break;
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
    void GUIWaitingPlayersState(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea("Waiting player to make their move");
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }
    void GUIProcessingState(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea("Server processing");
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }

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
    void WaitingPlayersState(){
        if (AllPlayersPlayed()){
            Debug.Log("SERVER - all players played");
            BoardManagerRef.CleanLastSoundEffects();
            nextState = ServerController.ServerState.Processing;
        }
    }
    void ProcessingState(){
        ProcessResults();
        nextState = ServerController.ServerState.Updating;
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

    void ProcessResults(){
        // Human escaped
        List<int> escapees = ProcessHumanEscapees();
        // Player kills another
        List<int> attacked = ProcessAttacks();
        // Update players stati
        UpdatePlayersStati(escapees, attacked);
        // Aliens delayed humans for 39 turns
        /// TODO turn countdown
    }

    void UpdatePlayersStati(List<int> escapess, List<int> attacked){
        foreach(int code in escapess){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(code, out data);
            data.role = ClientController.PlayerState.Escaped;
        }

        foreach(int code in attacked){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(code, out data);
            data.role = ClientController.PlayerState.Died;
        }
    }

    List<int> ProcessHumanEscapees(){
        List<string> escapePodCodes = BoardManagerRef.EscapePods;

        List<int> playersEscapees = new List<int>();
        foreach(int key in playerTurnDict.Keys){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;

            foreach(string code in escapePodCodes){
                Vector2Int escapePodePosition = BoardManager.TileCodeToVector2Int(code);

                if(data.role == ClientController.PlayerState.Human && lastPlay.movementTo == escapePodePosition){
                    playersEscapees.Add(lastPlay.playerId);
                    Debug.Log(string.Format("SERVER - player {0} escaped!", lastPlay.playerId));
                    break;
                }
            }
        }

        return playersEscapees;
    }

    List<int> ProcessAttacks(){
        // Get attacks positions
        List<Vector2Int> attackList = new List<Vector2Int>();
        foreach(int key in playerTurnDict.Keys){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;
            if(lastPlay.PlayerAttacked){
                attackList.Add(lastPlay.movementTo);
            }
        }

        // Check if players was in attack positions
        List<int> playersAttacked = new List<int>();
        foreach(int key in playerTurnDict.Keys){
            PlayerTurnData data;
            playerTurnDict.TryGetValue(key, out data);
            PutPlayRequest lastPlay = data.lastPlay;

            foreach(Vector2Int attackPosition in attackList){
                if(!lastPlay.PlayerAttacked && lastPlay.movementTo == attackPosition){
                    playersAttacked.Add(lastPlay.playerId);
                    Debug.Log(string.Format("SERVER - player {0} attacked!", lastPlay.playerId));
                    break;
                }
            }
        }

        return playersAttacked;
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