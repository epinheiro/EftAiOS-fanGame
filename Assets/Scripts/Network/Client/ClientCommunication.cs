using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;

public class ClientCommunication : MonoBehaviour
{
    int clientId;

    private UdpNetworkDriver m_ClientDriver;
    private NativeArray<NetworkConnection> m_clientToServerConnection;

    private CommunicationJobHandler jobHandler;

    NetworkEndPoint endpoint;

    public ClientController clientController;

    void Awake(){
        clientController = this.GetComponent<ClientController>();
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
        
        // Schedule a chain with the driver update followed by the other jobs
        jobHandler.ScheduleDriverUpdate(m_ClientDriver);

        ConnectionUpdateJob conUpdate = new ConnectionUpdateJob
        {
            driver = m_ClientDriver,
            connection = m_clientToServerConnection,
            serverEP = endpoint
        };
        jobHandler.QueueJob(conUpdate);

        jobHandler.ScheduleJobsInQueue();

        jobHandler.Complete();

        ProcessClientCommandCoroutine pcc = new ProcessClientCommandCoroutine(this, m_ClientDriver, jobHandler, m_clientToServerConnection[0]);
        m_clientToServerConnection[0] = pcc.connection;
    }

    public void SchedulePutPlayRequest(int clientId, Vector2Int movementTo, Vector2Int sound, bool attacked){
        PutPlayRequest request = new PutPlayRequest(clientId, movementTo.x,movementTo.y, sound.x,sound.y, attacked);
        IJob job = DataPackageWrapper.CreateSendDataJob(m_ClientDriver, m_clientToServerConnection[0], request.DataToArray());
        jobHandler.QueueJob(job);
    }

    public void ScheduleGetStateRequest(){
        GetStateRequest request = new GetStateRequest();
        IJob job = DataPackageWrapper.CreateSendDataJob(m_ClientDriver, m_clientToServerConnection[0], request.DataToArray());
        jobHandler.QueueJob(job);
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