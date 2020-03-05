using Unity.Networking.Transport;
using Unity.Collections;

public static class PlayerGetStateRequest
{
    static int CLASS_HARDCODED_BYTE_SIZE = 4;

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetState;

    static public DataStreamWriter PackRequest(){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);

        return writer;
    }
}
