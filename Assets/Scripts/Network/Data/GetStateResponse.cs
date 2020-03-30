using Unity.Networking.Transport;

public class GetStateResponse : INetworkData
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetState;
    public readonly int playerId;

    int _serverState;
    public ServerController.ServerState ServerState{
        get { return (ServerController.ServerState) _serverState; }
        set { _serverState = (int) value; }
    }

    public int[] DataToArray(){
        return new int[]{commandCode, playerId, _serverState};
    }

    /// <summary>
    /// This constructor is for WRAPPING the data to make a request
    /// </summary> 
    public GetStateResponse(int playerId, ServerController.ServerState serverState){
        this.playerId = playerId;
        this._serverState = (int) serverState;
    }

    /// <summary>
    /// This constructor is for UNWRAPPING the data to read a request
    /// </summary> 
    public GetStateResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
            this._serverState = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with this class command {1}", commandCheck, commandCode));
        }
    }
}

