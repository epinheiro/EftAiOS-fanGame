using Unity.Networking.Transport;
using System.Collections;
using UnityEngine;

public class ProcessCommandCoroutine<T> where T : MonoBehaviour
{
    public UdpNetworkDriver driver;
    public NetworkConnection connection;

    protected CommunicationJobHandler jobHandler;

    protected T owner;

    public ProcessCommandCoroutine(T owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler, NetworkConnection connection){
        this.driver = driver;
        this.connection = connection;
        this.jobHandler = jobHandler;
        this.owner = owner;

        owner.StartCoroutine(ProcessSingleConnection());
    } 

    public IEnumerator ProcessSingleConnection(){
        DataStreamReader strm;
        NetworkEvent.Type cmd;

        // Pop all events for the connection
        while ((cmd = driver.PopEventForConnection(connection, out strm)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                ConnectProcedure();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {                
                /////////////////////////////////////////////////////////////////////////
                ////////////////////////// RECEIVE DATA FROM CLIENT /////////////////////
                int command = ReadCommandReceived(strm);
                DataReceivedProcedure();
                ProcessCommandReceived(command, driver, connection, strm);
                ////////////////////////// SENT DATA BACK TO CLIENT /////////////////////
                /////////////////////////////////////////////////////////////////////////
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                // When disconnected we make sure the connection return false to IsCreated so the next frames
                // DriverUpdateJob will remove it
                DisconnectProcedure();
                connection.Disconnect(driver);
                connection.Close(driver);
                connection = default(NetworkConnection);
            }
        }

        yield return null;
    }

    int ReadCommandReceived(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
        int command = reader.ReadInt(ref readerCtx);

        try{
            return command;
        }catch{
            throw new System.Exception(string.Format("Command number {0} not found", command));
        }
    }

    protected virtual void ProcessCommandReceived(int command, UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        throw new System.Exception("ProcessCommandCoroutine child must implements own ProcessCommandReceived");
    }

    protected virtual void ConnectProcedure(){
        // throw new System.Exception("ProcessCommandCoroutine child must implements own ConnectProcedure");
    }

    protected virtual void DataReceivedProcedure(){
        // throw new System.Exception("ProcessCommandCoroutine child must implements own DataProcedure");
    }

    protected virtual void DisconnectProcedure(){
        // throw new System.Exception("ProcessCommandCoroutine child must implements own DisconnectProcedure");
    }


    struct PackageMetadata{
        public readonly int command;
        public readonly int id;

        public PackageMetadata(int command, int id){
            this.command = command;
            this.id = id;
        }
    }
}