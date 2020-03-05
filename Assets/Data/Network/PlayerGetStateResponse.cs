using Unity.Networking.Transport;

public class PlayerGetStateResponse
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetState;

    public readonly ServerController.ServerState serverState;

    public PlayerGetStateResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.serverState = (ServerController.ServerState) reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }
}
