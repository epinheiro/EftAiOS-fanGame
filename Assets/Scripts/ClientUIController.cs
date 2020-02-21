using System;
using UnityEngine;
using Unity.Networking.Transport;

/* The PingClientUIBehaviour is responsible for displaying statistics of a
 running ping client as well as for starting and stopping the ping of a
 selected ip. */
public class ClientUIController : MonoBehaviour
{
    // The EndPoint the ping client should ping, will be a non-created end point when ping should not run.
    public static NetworkEndPoint ServerEndPoint { get; private set; }

    // Ping statistics
    static int s_PingTime;
    static int s_PingCounter;

    void Start(){
        s_PingTime = 0;
        s_PingCounter = 0;
        ServerEndPoint = default(NetworkEndPoint);
    }

    void OnGUI(){
        UpdatePingClientUI();
    }

    // Update the ping statistics displayed in the ui. Should be called from the ping client every time a new ping is complete
    public static void UpdateStats(int count, int time){
        s_PingCounter = count;
        s_PingTime = time;
    }

    void UpdatePingClientUI()
    {
        GUILayout.Label("PING " + s_PingCounter + ": " + s_PingTime + "ms");
        if (!ServerEndPoint.IsValid)
        {
            // Ping is not currently running, display ui for starting a ping
            if (GUILayout.Button("Start ping"))
            {
                ushort port = 9000;
                NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;
                endpoint.Port = port;
                ServerEndPoint = endpoint;
            }
        }
        else
        {
            // Ping is running, display ui for stopping it
            if (GUILayout.Button("Stop ping"))
            {
                ServerEndPoint = default(NetworkEndPoint);
            }
        }
    }
}
