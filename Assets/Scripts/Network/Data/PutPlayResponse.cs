using Unity.Networking.Transport;

public class PutPlayResponse : INetworkData
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.PutPlay;

    public int[] DataToArray(){
        return new int[]{commandCode};
    }

    /// <summary>
    /// This constructor is for WRAPPING the data to make a request
    /// </summary> 
    public PutPlayResponse(){
    }

    /// <summary>
    /// This constructor is for UNWRAPPING the data to read a request
    /// </summary> 
    public PutPlayResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck != commandCode){
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with this class command {1}", commandCheck, commandCode));
        }
    }
}