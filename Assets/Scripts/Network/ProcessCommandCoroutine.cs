using Unity.Networking.Transport;
using System.Collections;
using UnityEngine;
using System;

public class ProcessCommandCoroutine<T> where T : MonoBehaviour
{
    public UdpNetworkDriver driver;
    public NetworkConnection connection;

    protected CommunicationJobHandler jobHandler;

    BidirecionalIndex<int, int> connectionIndex;

    public int GetClientIdByInternalId(int internalId){
        return connectionIndex.GetByKey(internalId);
    }
    public int GetInteralIdByClientId(int clientId){
        return connectionIndex.GetByValue(clientId);
    }

    protected T owner;

    public ProcessCommandCoroutine(T owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler){
        this.driver = driver;
        this.jobHandler = jobHandler;
        this.owner = owner;

        connectionIndex = new BidirecionalIndex<int, int>();
    }

    public void StartProcessCoroutine(NetworkConnection connection){
        this.connection = connection;
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
                ConnectProcedure(connection);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {                
                /////////////////////////////////////////////////////////////////////////
                ////////////////////////// RECEIVE DATA FROM CLIENT /////////////////////
                PackageMetadata metadata = ReadPackageMetadata(strm);
                ProcessId(connection.InternalId, metadata.id);
                DataReceivedProcedure(connection);
                ProcessCommandReceived(metadata.command, driver, connection, strm);
                ////////////////////////// SENT DATA BACK TO CLIENT /////////////////////
                /////////////////////////////////////////////////////////////////////////
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                // When disconnected we make sure the connection return false to IsCreated so the next frames
                // DriverUpdateJob will remove it
                DisconnectProcedure(connection);
                NodeDisconnection();
                connection.Disconnect(driver);
                connection.Close(driver);
                connection = default(NetworkConnection);
            }
        }

        yield return null;
    }

    void NodeDisconnection(){
        try{
            NodeLog(string.Format("{0}[{1}] disconnected", connectionIndex.GetByKey(connection.InternalId), connection.InternalId));
            connectionIndex.RemoveKey(connection.InternalId);
        }catch{
            NodeLog(string.Format("[{0}] disconnected", connection.InternalId));
        }
    }

    void NodeLog(string message){
        Type thisType = this.GetType();

        if(thisType == typeof(ProcessClientCommandCoroutine)){
            ClientLog(message);
        }else if(thisType == typeof(ProcessServerCommandCoroutine)){
           ServerLog(message);
        }else{
            throw new Exception(string.Format("Type {0} not valid", thisType));
        }
    }

    void ClientLog(string message){
        TimeLogger.Log("CLIENT - {0}", message);
    }

    void ServerLog(string message){
        TimeLogger.Log("SERVER - {0}", message);
    }

    PackageMetadata ReadPackageMetadata(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
        int command = reader.ReadInt(ref readerCtx);
        int id = reader.ReadInt(ref readerCtx);

        return new PackageMetadata(command, id);
    }

    void ProcessId(int internalId, int packageId){
        if(!connectionIndex.ContainsValue(packageId)){
            connectionIndex.Add(internalId, packageId);
        }
    }

    protected void ProcessCommandReceived(int enumCommandNumber, UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        ServerCommunication.ServerCommand command = (ServerCommunication.ServerCommand) enumCommandNumber;
        
        switch(command){
            case ServerCommunication.ServerCommand.PutPlay:
                PutPlayCommand(driver, connection, strm);
            break;
            case ServerCommunication.ServerCommand.GetState:
                GetStateCommand(driver, connection, strm);
            break;
            case ServerCommunication.ServerCommand.GetResults:
                GetResultsCommand(driver, connection, strm);
            break;
            default:
                throw new System.Exception(string.Format("Command number {0} not found", enumCommandNumber));
        }
    }

    protected virtual void PutPlayCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        throw new System.Exception("ProcessCommandCoroutine child must implements own PutPlayCommand");
    }
    protected virtual void GetStateCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        throw new System.Exception("ProcessCommandCoroutine child must implements own GetStateCommand");
    }
    protected virtual void GetResultsCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        throw new System.Exception("ProcessCommandCoroutine child must implements own GetResultsCommand");
    }

    protected virtual void ConnectProcedure(NetworkConnection connection){
        // throw new System.Exception("ProcessCommandCoroutine child must implements own ConnectProcedure");
    }

    protected virtual void DataReceivedProcedure(NetworkConnection connection){
        // throw new System.Exception("ProcessCommandCoroutine child must implements own DataProcedure");
    }

    protected virtual void DisconnectProcedure(NetworkConnection connection){
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