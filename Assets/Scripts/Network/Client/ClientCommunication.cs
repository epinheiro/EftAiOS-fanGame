using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System;

public class ClientCommunication : MonoBehaviour
{
    int clientId;

    private UdpNetworkDriver m_ClientDriver;
    private NativeArray<NetworkConnection> m_clientToServerConnection;

    private CommunicationJobHandler jobHandler;

    NetworkEndPoint endpoint;

    void Awake(){
        jobHandler = new CommunicationJobHandler();
        SetClientIdentity();
    }

    void Start(){        
        ConnectToServer();
    }

    void OnDestroy(){
        // All jobs must be completed before we can dispose the data they use
        jobHandler.Complete();
        m_ClientDriver.Disconnect(m_clientToServerConnection[0]);
        m_ClientDriver.Dispose();
        m_clientToServerConnection.Dispose();
    }

    void LateUpdate(){
        // On fast clients we can get more than 4 frames per fixed update, this call prevents warnings about TempJob
        // allocation longer than 4 frames in those cases
        jobHandler.Complete();
    }

    void FixedUpdate(){
        // Wait for the previous frames ping to complete before starting a new one, the Complete in LateUpdate is not
        // enough since we can get multiple FixedUpdate per frame on slow clients
        jobHandler.Complete();

        // Schedule a chain with the driver update followed by the other jobs
        jobHandler.ScheduleDriverUpdate(m_ClientDriver);

        ConnectionUpdateJob conUpdate = new ConnectionUpdateJob
        {
            driver = m_ClientDriver,
            connection = m_clientToServerConnection,
            serverEP = endpoint
        };
        jobHandler.QueueJob(conUpdate);

        ProcessDataJob processData = new ProcessDataJob
        {
            driver = m_ClientDriver,
            connection = m_clientToServerConnection,
            clientId = clientId
        };
        jobHandler.QueueJob(processData);

        jobHandler.ScheduleJobsInQueue();
    }

    //////////////////////////////////
    /////// Client functions /////////
    void ConnectToServer(string ip = "", ushort port = 0){
        AllocateServerAttributes();

        if(string.IsNullOrEmpty(ip)){
            m_clientToServerConnection[0] = m_ClientDriver.Connect(GenerateNetworkEndPoint());
        }else{
            if (port == 0){
                m_clientToServerConnection[0] = m_ClientDriver.Connect(GenerateNetworkEndPoint(ip));
            }else{
                m_clientToServerConnection[0] = m_ClientDriver.Connect(GenerateNetworkEndPoint(ip, port));
            }
        }
    }

    void AllocateServerAttributes(){
        m_ClientDriver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_clientToServerConnection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
    }

    NetworkEndPoint GenerateNetworkEndPoint(){
        NetworkEndPoint outNet;
        outNet = NetworkEndPoint.LoopbackIpv4;
        outNet.Port = 9000;

        endpoint = outNet;

        return outNet;
    }

    NetworkEndPoint GenerateNetworkEndPoint(string ip, ushort port = 9000){
        NetworkEndPoint outNet;
        NetworkEndPoint.TryParse(ip, port, out outNet);

        endpoint = outNet;

        return outNet;
    }

    public void SetClientIdentity(){
        clientId = this.GetComponent<ClientController>().ClientId;
    }
}

[BurstCompile]
struct ConnectionUpdateJob : IJob{
    public UdpNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NetworkEndPoint serverEP;

    public void Execute()
    {
        // If the client ui indicates we should be sending pings but we do not have an active connection we create one
        if (serverEP.IsValid && !connection[0].IsCreated){
            Debug.Log("Client reconnection");
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

[BurstCompile]
struct ProcessDataJob : IJob{
    public UdpNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public int clientId;

    public void Execute()
    {
        if(connection[0].IsCreated){
            DataStreamReader strm;
            NetworkEvent.Type cmd;
            // Process all events on the connection. If the connection is invalid it will return Empty immediately
            while ((cmd = connection[0].PopEvent(driver, out strm)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    // /////////////////////////////////////////////////////////////////////////
                    // ////////////////////////// SEND DATA TO SERVER /////////////////////
                    PutPlayRequest request = new PutPlayRequest(clientId, 66,66, 44,44, false);

                    // Because of the type of allocation of DataPackage - DEBUG/TEST behaviour is made by hand
                    NativeArray<int> array = new NativeArray<int>(request.DataToArray(), Allocator.Temp);
                    SendDataJob sendData = new SendDataJob{
                        driver = driver,
                        connection = connection[0],
                        varargs = array
                    };
                    // Because of the type of allocation of DataPackage - DEBUG/TEST behaviour is made by hand

                    sendData.Execute();
                    // ////////////////////////// SEND DATA TO SERVER /////////////////////
                    // /////////////////////////////////////////////////////////////////////////

                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    /////////////////////////////////////////////////////////////////////////
                    ////////////////////////// RECEIVE DATA FROM SERVER /////////////////////
                    PutPlayResponse responseReceived = new PutPlayResponse(strm);

                    Debug.Log(string.Format("CLIENT received response - {0}", 
                        (ServerCommunication.ServerCommand) PutPlayResponse.commandCode)); // DEBUG METHOD TO CHECK COMMUNICATION
                    ////////////////////////// RECEIVE DATA FROM SERVER /////////////////////
                    /////////////////////////////////////////////////////////////////////////

                    // When the pong message is received we calculate the ping time and disconnect
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    // If the server disconnected us we clear out connection
                    connection[0] = default(NetworkConnection);
                }
            }
        }
    }
        
}