using Unity.Networking.Transport;

public class GetStateResponse : INetworkData
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetState;
    int _serverState;
    public ServerController.ServerState ServerState{
        get { return (ServerController.ServerState) _serverState; }
        set { _serverState = (int) value; }
    }

    public int[] DataToArray(){
        return new int[]{commandCode, _serverState};
    }

    /// <summary>
    /// This constructor is for WRAPPING the data to make a request
    /// </summary> 
    public GetStateResponse(ServerController.ServerState serverState){
        this._serverState = (int) serverState;
    }

    /// <summary>
    /// This constructor is for UNWRAPPING the data to read a request
    /// </summary> 
    public GetStateResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this._serverState = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with this class command {1}", commandCheck, commandCode));
        }
    }
}

