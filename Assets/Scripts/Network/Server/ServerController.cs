using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using Unity.Networking.Transport;
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

    ServerState currentState = ServerState.SetUp;

    ServerCommunication serverCommunication;
    string serverIp;


    void Start(){
        serverCommunication = gameObject.AddComponent(typeof(ServerCommunication)) as ServerCommunication;
        serverIp = GetLocalIPAddress();
    }

    void OnGUI(){
        if (currentState == ServerState.SetUp){
            // DEBUG positioning
            GUILayout.BeginArea(new Rect(100, 100, 175, 175));
            // DEBUG positioning

            GUILayout.TextArea(string.Format("Connect to IP: {0}", serverIp));
            
            // DEBUG positioning
            GUILayout.EndArea();
            // DEBUG positioning
        }
    }

    // Update is called once per frame
    void Update(){
        switch(currentState){
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
