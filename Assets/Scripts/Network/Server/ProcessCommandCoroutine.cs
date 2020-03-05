using Unity.Networking.Transport;
using System.Collections;
using UnityEngine;

public class ProcessCommandCoroutine
{
    public UdpNetworkDriver driver;
    public NetworkConnection connection;

    public ProcessCommandCoroutine(MonoBehaviour owner, UdpNetworkDriver driver, NetworkConnection connection){
        this.driver = driver;
        this.connection = connection;

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
        PlayerTurnDataRequest dataFromClient = new PlayerTurnDataRequest(strm);

        DataStreamWriter dataToClient = dataFromClient.PackPlayerTurnObjectData();

        driver.Send(NetworkPipeline.Null, connection, dataToClient);
    }

    void GetStateCommand(){

    }

    void GetResults(){

    }
}
