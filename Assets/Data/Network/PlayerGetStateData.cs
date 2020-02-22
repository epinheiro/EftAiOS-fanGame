using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerGetStateData : MonoBehaviour
{
    static int CLASS_HARDCODED_BYTE_SIZE = 12;


    static public readonly int commandCode = (int) ServerController.ServerCommand.GetResults;
    public readonly int playerId;

    public int playerAlive;

    public PlayerGetStateData(int playerId, int isAlive){
        this.playerId = playerId;
        this.playerAlive = isAlive;
    }

    public PlayerGetStateData(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
            this.playerAlive = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }

    static public DataStreamWriter CreateAndPackPlayerGetStateData(int playerId, int isAlive){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);
        writer.Write(playerId);
        writer.Write(isAlive);

        return writer;
    }

    public DataStreamWriter PackPlayerTurnObjectData(){
        return PlayerGetStateData.CreateAndPackPlayerGetStateData(playerId, playerAlive);
    }

    public override string ToString(){
        return string.Format("Command {0} : Player{1} - Alive{2})",
        (ServerController.ServerCommand) commandCode, playerId, playerAlive);
    }
}