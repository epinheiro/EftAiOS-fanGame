using System;
using Unity.Networking.Transport;
using UnityEngine;

public class ProcessClientCommandCoroutine : ProcessCommandCoroutine<ClientCommunication>
{
    public Action<ServerController.ServerState> GetStateEvent;
    public Action<ClientController.PlayerState, Vector2Int, PlayerTurnData.UIColors> GetResultsEvent;

    public ProcessClientCommandCoroutine(ClientCommunication owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler) :
        base(owner, driver, jobHandler){
    }

    protected override void PutPlayCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        PutPlayResponseData responseReceived = new PutPlayResponseData(strm);

        TimeLogger.Log("CLIENT {0} - response - PutPlay success", ((ClientCommunication)owner).ClientId);
    }

    protected override void GetStateCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetStateResponseData responseReceived = new GetStateResponseData(strm);

        //TimeLogger.Log("CLIENT {0} - response - GetState ({1})", ((ClientCommunication)owner).ClientId, responseReceived.ServerState);
        GetStateEvent?.Invoke(responseReceived.ServerState);
    }

    protected override void GetResultsCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetResultsResponseData responseReceived = new GetResultsResponseData(strm);

        ClientController.PlayerState playerState = (ClientController.PlayerState) responseReceived.playerState;
        PlayerTurnData.UIColors playerColor = (PlayerTurnData.UIColors) responseReceived.playerColor;

        TimeLogger.Log("CLIENT {0} - {3} - response - GetResults ({1} at {2})",
            ((ClientCommunication)owner).ClientId, playerState, responseReceived.playerPosition,  playerColor);

        GetResultsEvent?.Invoke(playerState, responseReceived.playerPosition, playerColor);
    }

    protected  override void ConnectProcedure(NetworkConnection connection){
        TimeLogger.Log("CLIENT {0} - connected", owner.ClientId);
    }

    protected  override void DisconnectProcedure(NetworkConnection connection){
        TimeLogger.Log("CLIENT {0} - disconnected", owner.ClientId);
    }
}
