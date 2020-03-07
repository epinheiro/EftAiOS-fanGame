using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerTurnDataResponse{

    static int CLASS_HARDCODED_BYTE_SIZE = 8;

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.PutPlay;
    public readonly int playerId;

    public PlayerTurnDataResponse(int playerId, int newPositionX, int newPositionY, int soundX, int soundY, int playerAttacked){
        this.playerId = playerId;
    }

    public PlayerTurnDataResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }

    static public DataStreamWriter CreateAndPackPlayerTurnData(int playerId){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);
        writer.Write(playerId);

        return writer;
    }

    public DataStreamWriter PackPlayerTurnObjectData(){
        return PlayerTurnDataResponse.CreateAndPackPlayerTurnData(this.playerId);
    }
}