using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;


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


    void Start(){
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
                ProcessingState();
            break;
            case ServerState.Updating:
                // Update the board
                UpdatingState();
            break;
            
        }
    }

    //////// On GUI methods
    void GUISetUpState(){
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
    
    //////// Update logic methods
    void WaitingPlayersState(){

    }
    void ProcessingState(){

    }
    void UpdatingState(){

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
