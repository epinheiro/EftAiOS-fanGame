using Unity.Networking.Transport;
using UnityEngine;
using Unity.Jobs;
using System;

public class ProcessServerCommandCoroutine : ProcessCommandCoroutine<ServerCommunication>
{
    public Action<PutPlayRequestData> PutPlayEvent;

    public ProcessServerCommandCoroutine(ServerCommunication owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler) :
        base(owner, driver, jobHandler){
    }

    protected override  void PutPlayCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        PutPlayRequestData requestReceived = new PutPlayRequestData(strm);
        TimeLogger.Log("SERVER - {0}[{1}] request - PutPlay (({2:00},{3:00}) ({4:00},{5:00}) ({6}))",
        requestReceived.playerId, connection.InternalId, requestReceived.movementTo.x, requestReceived.movementTo.y, requestReceived.sound.x, requestReceived.sound.y, requestReceived.PlayerAttacked);

        PutPlayEvent?.Invoke(requestReceived);

        PutPlayResponseData response = new PutPlayResponseData(requestReceived.playerId);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    protected override  void GetStateCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetStateRequestData requestReceived = new GetStateRequestData(strm);
        int clientId = requestReceived.playerId;

        ServerController.ServerState currentServerState = ((ServerCommunication)owner).serverController.CurrentState;

        //TimeLogger.Log("SERVER - {0}[{1}] request - GetState ({2})", clientId, connection.InternalId, currentServerState);

        GetStateResponseData response = new GetStateResponseData(clientId, currentServerState);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    protected override  void GetResultsCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetResultsRequestData requestReceived = new GetResultsRequestData(strm);
        int clientId = requestReceived.playerId;

        int playerColor;
        Vector2Int position;
        ClientController.PlayerState state;
        ((ServerCommunication)owner).serverController.GetPlayerData(clientId, out playerColor, out position, out state);

        TimeLogger.Log("SERVER - {0}[{1}] - {3} - request - GetResults ({2})", clientId, connection.InternalId, state, (PlayerTurnData.UIColors) playerColor);

        GetResultsResponseData response = new GetResultsResponseData(clientId, playerColor, state, position);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    protected  override void DisconnectProcedure(NetworkConnection connection){
        if(((ServerCommunication)owner).serverController.CurrentState == ServerController.ServerState.WaitingPlayers){
            int internalId = connection.InternalId;
            int clientId = GetClientIdByInternalId(internalId);
            owner.ClientDisconnection(clientId);
        }
    }
}
