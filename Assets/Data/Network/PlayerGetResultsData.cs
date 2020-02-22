using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerGetResultsData
{
    static int CLASS_HARDCODED_BYTE_SIZE = 8;


    static public readonly int commandCode = (int) ServerController.ServerCommand.GetState;
    public readonly int playerId;

    public PlayerGetResultsData(int playerId){
        this.playerId = playerId;
    }

    public PlayerGetResultsData(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }

    static public DataStreamWriter CreateAndPackPlayerGetResultsData(int playerId){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);
        writer.Write(playerId);

        return writer;
    }

    public DataStreamWriter PackPlayerTurnObjectData(){
        return PlayerGetResultsData.CreateAndPackPlayerGetResultsData(playerId);
    }

    public override string ToString(){
        return string.Format("Command {0} : Player{1})",
        (ServerController.ServerCommand) commandCode, playerId);
    }
}
