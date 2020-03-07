using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerGetResultResponse
{

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.GetResults;
    public readonly int playerId;
    public readonly int playerIsAlive;

    public PlayerGetResultResponse(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
            this.playerIsAlive = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }
}
