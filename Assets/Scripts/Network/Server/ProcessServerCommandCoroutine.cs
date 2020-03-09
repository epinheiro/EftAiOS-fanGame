using Unity.Networking.Transport;
using UnityEngine;
using Unity.Jobs;

public class ProcessServerCommandCoroutine : ProcessCommandCoroutine<ServerCommunication>
{
    public ProcessServerCommandCoroutine(ServerCommunication owner, UdpNetworkDriver driver, CommunicationJobHandler jobHandler, NetworkConnection connection) : 
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
                GetResults();
            break;
            default:
                throw new System.Exception(string.Format("Command number {0} not found", enumCommandNumber));
        }
    }

    void PutPlayCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        PutPlayRequest requestReceived = new PutPlayRequest(strm);
        Debug.Log(string.Format("SERVER receive request - PutPlay ({0}) ({1},{2}) ({3},{4}) ({5})", 
        requestReceived.playerId, requestReceived.movementTo.x, requestReceived.movementTo.y, requestReceived.sound.x, requestReceived.sound.y, requestReceived.PlayerAttacked));

        PutPlayResponse response = new PutPlayResponse();
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    void GetStateCommand(UdpNetworkDriver driver, NetworkConnection connection, DataStreamReader strm){
        GetStateRequest requestReceived = new GetStateRequest(strm);
        Debug.Log("SERVER receive request - GetState");

        GetStateResponse response = new GetStateResponse(((ServerCommunication)owner).serverController.CurrentState);
        IJob job = DataPackageWrapper.CreateSendDataJob(driver, connection, response.DataToArray());
        jobHandler.QueueJob(job);
    }

    void GetResults(){

    }
}
