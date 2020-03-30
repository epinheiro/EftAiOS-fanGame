using Unity.Networking.Transport;
using System.Collections;
using UnityEngine;

public class ProcessClientCommandCoroutine : ProcessCommandCoroutine<ClientCommunication>
{
    public ProcessClientCommandCoroutine(ClientCommunication owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler, NetworkConnection connection) : 
        base(owner, driver, jobHandler, connection){
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
        PutPlayResponse responseReceived = new PutPlayResponse(strm);

        Debug.Log(string.Format("CLIENT {0} - response - PutPlay success", ((ClientCommunication)owner).ClientId));
    }

    void GetStateCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetStateResponse responseReceived = new GetStateResponse(strm);

        Debug.Log(string.Format("CLIENT {0} - response - GetState ({1})", ((ClientCommunication)owner).ClientId, responseReceived.ServerState));
        ((ClientCommunication)owner).clientController.ServerState = responseReceived.ServerState;
    }

    void GetResults(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetResultsResponse responseReceived = new GetResultsResponse(strm);

        ClientController.PlayerState playerState = (ClientController.PlayerState) responseReceived.playerState;
        Debug.Log(string.Format("CLIENT {0} - response - GetResults ({1} at {2})", 
            ((ClientCommunication)owner).ClientId, playerState, responseReceived.playerPosition));
        
        ((ClientCommunication)owner).clientController.NextPlayerState = playerState;
        ((ClientCommunication)owner).clientController.playerCurrentPosition = responseReceived.playerPosition;
    }
}
