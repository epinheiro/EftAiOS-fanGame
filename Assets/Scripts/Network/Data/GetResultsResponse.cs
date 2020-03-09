using UnityEngine;
using Unity.Networking.Transport;

public class GetResultsResponse : INetworkData
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetResults;

    public int playerState; // TODO proper enum of player states

    public int playerRole; // TODO proper enum of player roles

    public readonly Vector2Int playerPosition; 

    public int[] DataToArray(){
        return new int[]{commandCode, playerState};
    }

    /// <summary>
    /// This constructor is for WRAPPING the data to make a request
    /// </summary> 
    public GetResultsResponse(int playerState, int playerPositionX, int playerPositionY){
        this.playerState = playerState;
        playerPosition = new Vector2Int(playerPositionX, playerPositionY);
    }

    /// <summary>
    /// This constructor is for UNWRAPPING the data to read a request
    /// </summary> 
    public GetResultsResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerState = reader.ReadInt(ref readerCtx);
            playerPosition = new Vector2Int(reader.ReadInt(ref readerCtx), reader.ReadInt(ref readerCtx));
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with this class command {1}", commandCheck, commandCode));
        }
    }
}