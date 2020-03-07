using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerGetStateRequest : INetworkData
{
    static int CLASS_HARDCODED_BYTE_SIZE = 4;

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetState;

    public DataStreamWriter PackData(){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);

        return writer;
    }

    public int[] DataToArray(){
        return null;
    }    
}
