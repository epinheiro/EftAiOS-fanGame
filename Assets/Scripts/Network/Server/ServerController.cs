using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class ServerController : MonoBehaviour
{
    public enum ServerState {
        // Pre-game states
        SetUp, 
        // In-game states
        WaitingPlayers, 
        Processing, 
        Updating
    }

    ServerState _currentState = ServerState.SetUp;
    public ServerState CurrentState{
        get { return _currentState; }
    }

    ServerState nextState = ServerState.SetUp;

    ServerCommunication serverCommunication;
    string serverIp;

    Dictionary<int, PlayerTurnData> playerTurnDict;

    ExtendedList<ClientController.PlayerState> playerRolesToGive;

    void Start(){
        playerRolesToGive = new ExtendedList<ClientController.PlayerState>();
        playerTurnDict = new Dictionary<int, PlayerTurnData>();

        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;
        serverIp = GetLocalIPAddress();
    }

    void OnGUI(){
        switch(_currentState){
            case ServerState.SetUp:
                GUISetUpState();
            break;

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
            finalPosition = new Vector2Int(playerId, playerId); // TODO get proper SPAWN point

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
    void GUISetUpState(){
        // DEBUG positioning
        GUILayout.BeginArea(new Rect(100, 100, 175, 175));
        // DEBUG positioning

        GUILayout.TextArea(string.Format("Connect to IP: {0}", serverIp));
        if (GUILayout.Button("Start game")){
            SetUpStateEnd();            
        }
        
        // DEBUG positioning
        GUILayout.EndArea();
        // DEBUG positioning
    }
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
    void SetUpStateEnd(){
        PreparePossibleRoles();
        nextState = ServerState.WaitingPlayers;
    }
    void PreparePossibleRoles(){
        int playersNumber = serverCommunication.ConnectionQuantity;
        int numberHalf = playersNumber/2;
        int alienModifier = playersNumber%2==0 ? 0 : 1; // There is always an even number of aliens - or 1 more

        playerRolesToGive.AddRedundant(ClientController.PlayerState.Alien, numberHalf + alienModifier);
        playerRolesToGive.AddRedundant(ClientController.PlayerState.Human, numberHalf);

        playerRolesToGive.Shuffle();
    }
    void WaitingPlayersState(){
        if (AllPlayersPlayed()){
            Debug.Log("SERVER - all players played");
            nextState = ServerController.ServerState.Processing;
        }
    }
    void ProcessingState(){
        nextState = ServerController.ServerState.Updating;
    }
    void UpdatingState(){
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

    // Based on the Stackoverflow answer https://stackoverflow.com/a/6803109
    public static string GetLocalIPAddress()
    {
        // https://docs.microsoft.com/pt-br/dotnet/api/system.text.regularexpressions.regex?view=netframework-4.8
        string pattern = @"^192.168.0.\d*";
        Regex rgx = new Regex(pattern);

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                string testedIp = ip.ToString();
                if(rgx.IsMatch(testedIp)){
                    return ip.ToString(); 
                }                
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}