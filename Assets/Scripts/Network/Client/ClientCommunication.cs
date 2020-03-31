using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;

public class ClientCommunication : NodeCommunication
{
    int _clientId;
    public int ClientId{
        get { return _clientId; }
    }

    private UdpNetworkDriver m_ClientDriver;
    private NativeArray<NetworkConnection> m_clientToServerConnection;

    private CommunicationJobHandler jobHandler;

    NetworkEndPoint endpoint;

    public ClientController clientController;

    public string _customIp = "";
    public string IP {
        get { return _customIp; }
        set { _customIp = value; }
    }

    public bool IsConnecting{
        get { return IsCertainConnectionState(NetworkConnection.State.Connected); }
    }
    public bool IsConnected{
        get { return IsCertainConnectionState(NetworkConnection.State.Connected); }
    }
    bool IsCertainConnectionState(NetworkConnection.State checkState){
        if(m_clientToServerConnection != null){
            try{
                NetworkConnection.State connState = m_clientToServerConnection[0].GetState(m_ClientDriver);
                if(connState == checkState){
                    return true;
                }
            }catch{}
        }
        return false;
    }
    public void Disconnect(){
        m_clientToServerConnection[0].Disconnect(m_ClientDriver);
        m_clientToServerConnection[0].Close(m_ClientDriver);
    }

    void Awake(){
        clientController = this.GetComponent<ClientController>();
        jobHandler = new CommunicationJobHandler();

        AllocateServerAttributes();
    }

    void Start(){
        SetClientIdentity();
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

        if(IsConnected){
            ProcessClientCommandCoroutine pcc = new ProcessClientCommandCoroutine(this, m_ClientDriver, jobHandler, m_clientToServerConnection[0]);
            m_clientToServerConnection[0] = pcc.connection;
        }
    }

    public void SchedulePutPlayRequest(Vector2Int movementTo, Vector2Int sound, bool attacked){
        int id = ClientId;
        int movX = movementTo.x;
        int movY = movementTo.y;
        int sndX = sound.x;
        int sndY = sound.y;
        bool atk = attacked;

        PutPlayRequest request = new PutPlayRequest(id, movX, movY, sndX, sndY, atk);
        TimeLogger.Log("CLIENT {0} - schedule request - PutPlay (({1:00},{2:00}) ({3:00},{4:00}) ({5}))", id, movX, movY, sndX, sndY, atk);

        IJob job = DataPackageWrapper.CreateSendDataJob(m_ClientDriver, m_clientToServerConnection[0], request.DataToArray());
        jobHandler.QueueJob(job);
    }

    public void ScheduleGetStateRequest(){
        int id = ClientId;

        GetStateRequest request = new GetStateRequest(ClientId);
        TimeLogger.Log("CLIENT {0} - schedule request - GetState", id);

        IJob job = DataPackageWrapper.CreateSendDataJob(m_ClientDriver, m_clientToServerConnection[0], request.DataToArray());
        jobHandler.QueueJob(job);
    }

    public void ScheduleGetResultsRequest(){
        int id = ClientId;

        GetResultsRequest request = new GetResultsRequest(id);
        TimeLogger.Log("CLIENT {0} - schedule request - GetResults", id);

        IJob job = DataPackageWrapper.CreateSendDataJob(m_ClientDriver, m_clientToServerConnection[0], request.DataToArray());
        jobHandler.QueueJob(job);
    }

    //////////////////////////////////
    /////// Client functions /////////
    public bool ConnectToServer(string ip = "", ushort port = 0){
        try{
            if(string.IsNullOrEmpty(ip)){
                m_clientToServerConnection[0] = m_ClientDriver.Connect(GenerateNetworkEndPoint());
            }else{
                if (port == 0){
                    m_clientToServerConnection[0] = m_ClientDriver.Connect(GenerateNetworkEndPoint(ip));
                }else{
                    m_clientToServerConnection[0] = m_ClientDriver.Connect(GenerateNetworkEndPoint(ip, port));
                }
            }
            return true;
        }catch (System.Exception e){
            TimeLogger.Log("CLIENT connection failure {0}", e.ToString());
            return false;
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
        _clientId = this.GetComponent<ClientController>().ClientId;
    }
}