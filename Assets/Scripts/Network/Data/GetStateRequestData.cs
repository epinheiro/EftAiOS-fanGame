﻿using Unity.Networking.Transport;

public class GetStateRequestData: INetworkData
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetState;
    public readonly int playerId;

    public int[] DataToArray(){
        return new int[]{commandCode, playerId};
    }

    /// <summary>
    /// This constructor is for WRAPPING the data to make a request
    /// </summary> 
    public GetStateRequestData(int playerId){
        this.playerId = playerId;
    }

    /// <summary>
    /// This constructor is for UNWRAPPING the data to read a request
    /// </summary> 
    public GetStateRequestData(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with this class command {1}", commandCheck, commandCode));
        }
    }
}