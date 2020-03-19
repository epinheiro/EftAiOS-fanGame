using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
struct ConnectionUpdateJob : IJob{
    public UdpNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NetworkEndPoint serverEP;

    public void Execute()
    {
        // If the client ui indicates we should be sending pings but we do not have an active connection we create one
        if (serverEP.IsValid && !connection[0].IsCreated){
            connection[0] = driver.Connect(serverEP);
        }
            
        // If the client ui indicates we should not be sending pings but we do have a connection we close that connection
        if (!serverEP.IsValid && connection[0].IsCreated)
        {
            connection[0].Disconnect(driver);
            connection[0] = default(NetworkConnection);
        }
    }
}