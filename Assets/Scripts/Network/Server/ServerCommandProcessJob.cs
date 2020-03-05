using Unity.Burst;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Jobs;

[BurstCompile]
struct ServerCommandProcessJob : IJobParallelForDefer{
    public UdpNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connections;

    public void Execute(int i)
    {
        connections[i] = ProcessSingleConnection(driver, connections[i]);
    }

    
    NetworkConnection ProcessSingleConnection(UdpNetworkDriver.Concurrent driver, NetworkConnection connection){
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
                return default(NetworkConnection);
            }
        }

        return connection;
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

    void ProcessCommandReceived(ServerCommunication.ServerCommand command, UdpNetworkDriver.Concurrent driver, NetworkConnection connection, DataStreamReader strm){
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

    void PutPlayCommand(UdpNetworkDriver.Concurrent driver, NetworkConnection connection, DataStreamReader strm){
        PlayerTurnDataRequest dataFromClient = new PlayerTurnDataRequest(strm);

        DataStreamWriter dataToClient = dataFromClient.PackPlayerTurnObjectData();

        driver.Send(NetworkPipeline.Null, connection, dataToClient);
    }

    void GetStateCommand(){

    }

    void GetResults(){

    }

}
