using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerGetResultRequest
{
    static int CLASS_HARDCODED_BYTE_SIZE = 8;

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetResults;
    public readonly int playerId;

    static public DataStreamWriter CreateAndPackPlayerGetResultsData(int playerId, int playerAlive){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);
        writer.Write(playerId);

        return writer;
    }
}
