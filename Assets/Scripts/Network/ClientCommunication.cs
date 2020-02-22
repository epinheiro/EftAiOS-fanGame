using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using System.Text;  

public class ClientCommunication : MonoBehaviour
{
    public enum ClientState {Playing, WaitingPlayers, WaitingServer, Updating}

    ClientState currentState = ClientState.Updating;

    private UdpNetworkDriver m_ClientDriver;
    private NativeArray<NetworkConnection> m_clientToServerConnection;

    private JobHandle m_updateHandle;

    NetworkEndPoint endpoint;

    void Start(){
        ConnectToServer();
    }

    void OnDestroy(){
        // All jobs must be completed before we can dispose the data they use
        m_updateHandle.Complete();
        m_ClientDriver.Disconnect(m_clientToServerConnection[0]);
        m_ClientDriver.Dispose();
        m_clientToServerConnection.Dispose();
    }

    [BurstCompile]
    struct PingJob : IJob{
        public UdpNetworkDriver driver;
        public NativeArray<NetworkConnection> connection;
        public NetworkEndPoint serverEP;
        public float fixedTime;

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

            DataStreamReader strm;
            NetworkEvent.Type cmd;
            // Process all events on the connection. If the connection is invalid it will return Empty immediately
            while ((cmd = connection[0].PopEvent(driver, out strm)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("Client connection completed");
                    // /////////////////////////////////////////////////////////////////////////
                    // ////////////////////////// SEND DATA TO SERVER /////////////////////
                    DataStreamWriter pingData = PlayerTurnData.CreateAndPackPlayerTurnData(1, 2,2, 4,4);
                    connection[0].Send(driver, pingData);
                    // ////////////////////////// SEND DATA TO SERVER /////////////////////
                    // /////////////////////////////////////////////////////////////////////////

                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    /////////////////////////////////////////////////////////////////////////
                    ////////////////////////// RECEIVE DATA FROM SERVER /////////////////////
                    PlayerTurnData dataFromServer = new PlayerTurnData(strm);

                    Debug.Log(dataFromServer.ToString()); // DEBUG METHOD TO CHECK COMMUNICATION
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

    void LateUpdate(){
        // On fast clients we can get more than 4 frames per fixed update, this call prevents warnings about TempJob
        // allocation longer than 4 frames in those cases
        m_updateHandle.Complete();
    }

    void FixedUpdate(){
        // Wait for the previous frames ping to complete before starting a new one, the Complete in LateUpdate is not
        // enough since we can get multiple FixedUpdate per frame on slow clients
        m_updateHandle.Complete();

        // Update the ping client UI with the ping statistics computed by teh job scheduled previous frame since that
        // is now guaranteed to have completed
        PingJob pingJob = new PingJob
        {
            driver = m_ClientDriver,
            connection = m_clientToServerConnection,
            serverEP = endpoint,
            fixedTime = Time.fixedTime
        };
        // Schedule a chain with the driver update followed by the ping job
        m_updateHandle = m_ClientDriver.ScheduleUpdate();
        m_updateHandle = pingJob.Schedule(m_updateHandle);
    }

    //////////////////////////////////
    /////// Client functions /////////
    void ConnectToServer(){
        m_ClientDriver = new UdpNetworkDriver(new INetworkParameter[0]);

        m_clientToServerConnection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);

        endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;

        m_clientToServerConnection[0] = m_ClientDriver.Connect(endpoint);
    }
}
