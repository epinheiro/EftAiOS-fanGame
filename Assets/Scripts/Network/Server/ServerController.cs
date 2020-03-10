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

    Dictionary<int, PutPlayRequest> playersLastTurn;
    Dictionary<int, bool> playerTurnControl;

    void Start(){
        playersLastTurn = new Dictionary<int, PutPlayRequest>();
        playerTurnControl = new Dictionary<int, bool>();
        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;
        serverIp = GetLocalIPAddress();
    }

    void OnGUI(){
        switch(_currentState){
            case ServerState.SetUp:
                // DEBUG positioning
                GUILayout.BeginArea(new Rect(100, 100, 175, 175));
                // DEBUG positioning

                GUILayout.TextArea(string.Format("Connect to IP: {0}", serverIp));
                if (GUILayout.Button("Start game")){
                    nextState = ServerState.WaitingPlayers;
                }
                
                // DEBUG positioning
                GUILayout.EndArea();
                // DEBUG positioning
            break;

            case ServerState.WaitingPlayers:
                // DEBUG positioning
                GUILayout.BeginArea(new Rect(100, 100, 175, 175));
                // DEBUG positioning

                GUILayout.TextArea("Waiting player to make their move");
                
                // DEBUG positioning
                GUILayout.EndArea();
                // DEBUG positioning
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
            break;
            case ServerState.Processing:
                // Show "animation" of the turn
            break;
            case ServerState.Updating:
                // Update the board
            break;
            
        }
    }

    public void InsertNewPlayTurnData(int playerId, Vector2Int movementTo, Vector2Int soundIn, bool attacked){
        PutPlayRequest playerData = new PutPlayRequest(playerId, movementTo.x, movementTo.y, soundIn.x, soundIn.y, attacked);
        InsertNewPlayTurnData(playerData);
    }

    public void InsertNewPlayTurnData(PutPlayRequest putPlayData){
        bool playerAlreadyPlayed = false;
        playerTurnControl.TryGetValue(putPlayData.playerId, out playerAlreadyPlayed);

        if(!playerAlreadyPlayed){
            bool sucess = playersLastTurn.ContainsKey(putPlayData.playerId);
            if (sucess){
                playersLastTurn.Remove(putPlayData.playerId);
            }

            playersLastTurn.Add(putPlayData.playerId, putPlayData);
            UpdatePlayerTurnControl(true, putPlayData);
        }else{
            throw new System.Exception(string.Format("Player {0} already marked as PLAYED", putPlayData.playerId));
        }
    }

    public void UpdatePlayerTurnControl(bool alreadyPlay, PutPlayRequest playerTurnData){
        playerTurnControl.Remove(playerTurnData.playerId);
        playerTurnControl.Add(playerTurnData.playerId, alreadyPlay);
    }

    public void ResetPlayerTurnControl(){
        foreach(KeyValuePair<int, bool> entry in playerTurnControl){
            playerTurnControl[entry.Key] = false;
        }
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
