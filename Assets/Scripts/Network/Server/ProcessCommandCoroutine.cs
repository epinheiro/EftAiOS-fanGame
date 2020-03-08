using Unity.Networking.Transport;
using System.Collections;
using UnityEngine;
using Unity.Jobs;

public class ProcessCommandCoroutine
{
    public UdpNetworkDriver driver;
    public NetworkConnection connection;

    CommunicationJobHandler jobHandler;

    public ProcessCommandCoroutine(MonoBehaviour owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler, NetworkConnection connection){
        this.driver = driver;
        this.connection = connection;
        this.jobHandler = jobHandler;

        owner.StartCoroutine(ProcessSingleConnection());
    }

    public IEnumerator ProcessSingleConnection(){
        DataStreamReader strm;
        NetworkEvent.Type cmd;

        // Pop all events for the connection
        while ((cmd = driver.PopEventForConnection(connection, out strm)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {                
                /////////////////////////////////////////////////////////////////////////
                ////////////////////////// RECEIVE DATA FROM CLIENT /////////////////////
                ServerCommunication.ServerCommand command = ReadCommandReceived(strm);
                ProcessCommandReceived(command, driver, connection, strm);
                ////////////////////////// SENT DATA BACK TO CLIENT /////////////////////
                /////////////////////////////////////////////////////////////////////////
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                // When disconnected we make sure the connection return false to IsCreated so the next frames
                // DriverUpdateJob will remove it
                connection.Disconnect(driver);
                connection.Close(driver);
                connection = default(NetworkConnection);
            }
        }

        yield return null;
    }

    ServerCommunication.ServerCommand ReadCommandReceived(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
        int command = reader.ReadInt(ref readerCtx);

        try{
            return (ServerCommunication.ServerCommand) command;
        }catch{
            throw new System.Exception(string.Format("Command number {0} not found", command));
        }
    }

    void ProcessCommandReceived(ServerCommunication.ServerCommand command, UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        switch(command){
            case ServerCommunication.ServerCommand.PutPlay:
                PutPlayCommand(driver, connection, strm);
            break;
            case ServerCommunication.ServerCommand.GetState:
                GetStateCommand();
            break;
            case ServerCommunication.ServerCommand.GetResults:
                GetResults();
            break;
            default:
                throw new System.Exception(string.Format("Command number {0} not found", command));
        }
    }

    void PutPlayCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        PutPlayRequest requestReceived = new PutPlayRequest(strm);
        Debug.Log(string.Format("SERVER RECEIVE - PutPlay ({0}) ({1},{2}) ({3},{4}) ({5})", 
        requestReceived.playerId, requestReceived.movementTo.x, requestReceived.movementTo.y, requestReceived.sound.x, requestReceived.sound.y, requestReceived.PlayerAttacked));

        PutPlayRequest request = new PutPlayRequest( 66, 6,6, 7,7, false);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, request.DataToArray());
        jobHandler.QueueJob(job);
    }

    void GetStateCommand(){

    }

    void GetResults(){

    }
}
