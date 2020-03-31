using Unity.Networking.Transport;
using UnityEngine;
using Unity.Jobs;

public class ProcessServerCommandCoroutine : ProcessCommandCoroutine<ServerCommunication>
{
    public ProcessServerCommandCoroutine(ServerCommunication owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler) :
        base(owner, driver, jobHandler){
    }
    
    protected override void ProcessCommandReceived(int enumCommandNumber, UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        ServerCommunication.ServerCommand command = (ServerCommunication.ServerCommand) enumCommandNumber;
        
        switch(command){
            case ServerCommunication.ServerCommand.PutPlay:
                PutPlayCommand(driver, connection, strm);
            break;
            case ServerCommunication.ServerCommand.GetState:
                GetStateCommand(driver, connection, strm);
            break;
            case ServerCommunication.ServerCommand.GetResults:
                GetResults(driver, connection, strm);
            break;
            default:
                throw new System.Exception(string.Format("Command number {0} not found", enumCommandNumber));
        }
    }

    void PutPlayCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        PutPlayRequest requestReceived = new PutPlayRequest(strm);
        TimeLogger.Log("SERVER - {0}[{1}] request - PutPlay (({2:00},{3:00}) ({4:00},{5:00}) ({6}))",
        requestReceived.playerId, connection.InternalId, requestReceived.movementTo.x, requestReceived.movementTo.y, requestReceived.sound.x, requestReceived.sound.y, requestReceived.PlayerAttacked);

        ((ServerCommunication)owner).serverController.InsertNewPlayTurnData(requestReceived);

        PutPlayResponse response = new PutPlayResponse(requestReceived.playerId);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    void GetStateCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetStateRequest requestReceived = new GetStateRequest(strm);
        int clientId = requestReceived.playerId;

        ServerController.ServerState currentServerState = ((ServerCommunication)owner).serverController.CurrentState;

        TimeLogger.Log("SERVER - {0}[{1}] request - GetState ({2})", clientId, connection.InternalId, currentServerState);

        GetStateResponse response = new GetStateResponse(clientId, currentServerState);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    void GetResults(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetResultsRequest requestReceived = new GetResultsRequest(strm);
        int clientId = requestReceived.playerId;

        Vector2Int position;
        ClientController.PlayerState state;
        ((ServerCommunication)owner).serverController.GetPlayerData(clientId, out position, out state);

        TimeLogger.Log("SERVER - {0}[{1}] request - GetResults ({2})", clientId, connection.InternalId, state);

        GetResultsResponse response = new GetResultsResponse(clientId, state, position);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }
}
