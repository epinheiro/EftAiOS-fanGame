using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Jobs;


public class ServerCommunication : MonoBehaviour
{
    public enum ServerState {WaitingPlayers, Processing, Updating}
    public enum ServerCommand {PutPlay, GetState, GetResults}

    ServerState currentState = ServerState.Updating;

    public UdpNetworkDriver m_ServerDriver;
    private NativeList<NetworkConnection> m_connections;

    private JobHandle m_updateHandle;

    void Start(){
        InitServer();
    }

     void LateUpdate(){
        // On fast clients we can get more than 4 frames per fixed update, this call prevents warnings about TempJob
        // allocation longer than 4 frames in those cases
        m_updateHandle.Complete();
    }

    void OnDestroy(){
        // All jobs must be completed before we can dispose the data they use
        m_updateHandle.Complete();
        m_ServerDriver.Dispose();
        m_connections.Dispose();
    }

    void FixedUpdate(){
        // Wait for the previous frames ping to complete before starting a new one, the Complete in LateUpdate is not
        // enough since we can get multiple FixedUpdate per frame on slow clients
        m_updateHandle.Complete();
        DriverUpdateJob updateJob = new DriverUpdateJob {driver = m_ServerDriver, connections = m_connections};
        ServerCommandProcessJob commandJob = new ServerCommandProcessJob
        {
            // PongJob is a ParallelFor job, it must use the concurrent NetworkDriver
            driver = m_ServerDriver.ToConcurrent(),
            // PongJob uses IJobParallelForDeferExtensions, we *must* use AsDeferredJobArray in order to access the
            // list from the job
            connections = m_connections.AsDeferredJobArray()
        };
        // Update the driver should be the first job in the chain
        m_updateHandle = m_ServerDriver.ScheduleUpdate();
        // The DriverUpdateJob which accepts new connections should be the second job in the chain, it needs to depend
        // on the driver update job
        m_updateHandle = updateJob.Schedule(m_updateHandle);
        // PongJob uses IJobParallelForDeferExtensions, we *must* schedule with a list as first parameter rather than
        // an int since the job needs to pick up new connections from DriverUpdateJob
        // The PongJob is the last job in the chain and it must depends on the DriverUpdateJob
        m_updateHandle = commandJob.Schedule(m_connections, 1, m_updateHandle);
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
            Debug.Log($"Failed to bind to port {serverPort}");
        else
            m_ServerDriver.Listen();

        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    public ServerState GetState(){
        return currentState;
    }
}