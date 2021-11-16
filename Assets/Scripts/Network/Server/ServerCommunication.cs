using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerCommunication : NodeCommunication
{
    public Action<PutPlayRequestData> PutPlayEvent;

    public enum ServerCommand {PutPlay, GetState, GetResults}

    public UdpNetworkDriver m_ServerDriver;
    private NativeList<NetworkConnection> m_connections;
    public int ConnectionQuantity{
        get { return m_connections.Length; }
    }

    private CommunicationJobHandler jobHandler;

    ProcessServerCommandCoroutine pcc;

    ServerController serverController;

    public ServerController.ServerState ServerCurrentState => serverController.CurrentState;

    public PlayerSimplifiedTurnData GetPlayerData(int playerId){
        return serverController.GetPlayerData(playerId);
    }

    public void ClientDisconnection(int clientId){
        serverController.PlayerDisconnection(clientId);
    }

    void Awake(){
        InitServer();
        SetUp();
    }

    void SetUp(){
        serverController = this.GetComponent<ServerController>();
        jobHandler = new CommunicationJobHandler();

        pcc = new ProcessServerCommandCoroutine(this, m_ServerDriver, jobHandler);

        pcc.PutPlayEvent += PutPlayEventCallback;
    }

    void PutPlayEventCallback(PutPlayRequestData requestReceived){
        PutPlayEvent?.Invoke(requestReceived);
    }

     void LateUpdate(){
        // On fast clients we can get more than 4 frames per fixed update, this call prevents warnings about TempJob
        // allocation longer than 4 frames in those cases
        jobHandler.Complete();
    }

    void OnDestroy(){
        // All jobs must be completed before we can dispose the data they use
        jobHandler.Complete();
        m_ServerDriver.Dispose();
        m_connections.Dispose();

        pcc.PutPlayEvent -= PutPlayEventCallback;
    }

    void FixedUpdate(){
        // Wait for the previous frames ping to complete before starting a new one, the Complete in LateUpdate is not
        // enough since we can get multiple FixedUpdate per frame on slow clients
        
        DriverUpdateJob updateJob = new DriverUpdateJob {
            driver = m_ServerDriver, 
            connections = m_connections,
            serverState = serverController.CurrentState
        };
        // Update the driver should be the first job in the chain
        jobHandler.ScheduleDriverUpdate(m_ServerDriver);
        // The DriverUpdateJob which accepts new connections should be the second job in the chain, it needs to depend
        // on the driver update job
        jobHandler.QueueJob(updateJob);
        // PongJob uses IJobParallelForDeferExtensions, we *must* schedule with a list as first parameter rather than
        // an int since the job needs to pick up new connections from DriverUpdateJob
        // The PongJob is the last job in the chain and it must depends on the DriverUpdateJob

        jobHandler.ScheduleJobsInQueue();

        jobHandler.Complete();

        for(int i=0 ; i<m_connections.Length ; i++){
            pcc.StartProcessCoroutine(m_connections[i]);
            m_connections[i] = pcc.connection;
        }
    }

    //////////////////////////////////
    /////// Server functions /////////
    void InitServer(){
        ushort serverPort = 9000;
        // Create the server driver, bind it to a port and start listening for incoming connections
        m_ServerDriver = new UdpNetworkDriver(new INetworkParameter[0]);
        NetworkEndPoint addr = NetworkEndPoint.AnyIpv4;
        addr.Port = serverPort;
        if (m_ServerDriver.Bind(addr) != 0)
            TimeLogger.Log($"Failed to bind to port {serverPort}");
        else
            m_ServerDriver.Listen();

        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }
}