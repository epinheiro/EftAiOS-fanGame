﻿using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Jobs;

[BurstCompile]
struct DriverUpdateJob : IJob{
    public UdpNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public ServerController.ServerState serverState;

    public void Execute()
    {
        // Remove connections which have been destroyed from the list of active connections
        for (int i = 0; i < connections.Length; ++i)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                // Index i is a new connection since we did a swap back, check it again
                --i;
            }
        }

        // Accept all new connections when server is on SET UP state
        if(serverState == ServerController.ServerState.SetUp){
            while (true)
            {
                NetworkConnection con = driver.Accept();
                // "Nothing more to accept" is signaled by returning an invalid connection from accept
                if (con.IsCreated){
                    // TimeLogger.Log("SERVER - new connection - [{0}]", con.InternalId);
                    connections.Add(con);
                }else{
                    break;
                }
            }
            
        }
    }
}